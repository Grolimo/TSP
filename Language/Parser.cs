// Copyright (c) 2020-2021 Marco Caspers. All Rights Reserved.
// Copyright (c) 2021 DiBiAsi Software.
// Licensed under the Mozila Public License, version 2.0.

using System.Collections.Generic;

namespace Language
{
    public class Parser
    {
        private readonly List<string> Keywords = new List<string> { "break", "elif", "else", "foreach", "halt", "if", "unset", "while" };
        private readonly List<string> BoolOperands = new List<string> { "and", "not", "or", "xor" };
        private readonly List<string> VariableTypes = new List<string> { "array", "bool", "float", "int", "nil", "record", "string" };
        private static readonly Dictionary<string, KeyValuePair<int, string>> precedenceTable =
            new Dictionary<string, KeyValuePair<int, string>>
        {
            {"(", new KeyValuePair<int, string>(10, "left")},
            {"<", new KeyValuePair<int,string>(9, "left")},
            {">", new KeyValuePair<int,string>(9, "left")},
            {"<=", new KeyValuePair<int,string>(9, "left")},
            {">=", new KeyValuePair<int,string>(9, "left")},
            {"==", new KeyValuePair<int,string>(8, "left")},
            {"!=", new KeyValuePair<int,string>(8, "left")},
            {"^", new KeyValuePair<int,string>(7, "right")},
            {"/", new KeyValuePair<int,string>(6, "left")},
            {"*", new KeyValuePair<int,string>(6, "left")},
            {"+", new KeyValuePair<int,string>(5, "left")},
            {"-", new KeyValuePair<int,string>(5, "left")},
            {"not", new KeyValuePair<int,string>(4, "unary")},
            {"and", new KeyValuePair<int,string>(1, "left")},
            {"xor", new KeyValuePair<int,string>(1, "left")},
            {"or", new KeyValuePair<int,string>(1, "left")},
            {")", new KeyValuePair<int,string>(0, "left")},
        };

        private static readonly List<TokenType> Operands = new List<TokenType>
            { TokenType.OpPower, TokenType.OpMultiply, TokenType.OpDivide, TokenType.OpAdd, TokenType.OpSubtract,
                TokenType.OpLT, TokenType.OpLTE, TokenType.OpGT, TokenType.OpGTE, TokenType.OpEqual, TokenType.OpNE,
                TokenType.OpNot };

        private static readonly List<TokenType> Constants = new List<TokenType>
            {
                TokenType.ConstantString,
                TokenType.FormatString,
                TokenType.ConstantNumber,
                TokenType.ConstantBool,
                TokenType.ConstantNil,
                TokenType.ConstantRecord,
                TokenType.ConstantArray
            };
        public Ast_Application Root;

        private readonly Libraries Libraries;

        private const string ste_InvalidTokenType = "Invalid token type ({0}).";

        private static Token Expect(TokenType type, ParserState state)
        {
            var token = state.GetToken();
            if (token.Type != type)
            {
                throw new SyntaxError(token, string.Format(ste_InvalidTokenType, token.Type));
            }
            return token;
        }
        private static Token Expect(List<TokenType> type, ParserState state)
        {
            var token = state.GetToken();
            if (!type.Contains(token.Type))
            {
                throw new SyntaxError(token, string.Format(ste_InvalidTokenType, token.Type));
            }
            return token;
        }

        public Parser(Libraries libraries)
        {
            Libraries = libraries;
            Root = new Ast_Application { Libraries = libraries };
        }

        public void GetAst(IAst node, ParserState state)
        {
            var token = state.PeekToken();
            while (token != null && token.Type != TokenType.EOF)
            {
                if (token.Type == TokenType.BlockLeft)
                {
                    node?.Block.Add(ParseBlock(state));
                }
                else if (token.Type == TokenType.BlockRight)
                {
                    break;
                }
                else if (token.Type == TokenType.Identifier)
                {
                    node?.Block.Add(Parse(state));
                }
                else if (token.Type == TokenType.Comment)
                {
                    _ = state.GetToken();
                    // Comments are ignored, so just gobble it up.
                }
                else
                {

                    throw new SyntaxError(token, $"Invalid token type ({token.Type}), expected block or identifier.");
                }
                token = state.PeekToken();
            }
        }

        public Ast_Base Parse(ParserState state)
        {
            var token = Expect(TokenType.Identifier, state);
            var ident = ParseIdentifier(token, state);
            Ast_Base ast = ident.Type switch
            {
                AstType.Call => ParseCall(ident, state),
                AstType.Keyword => ParseKeyword(ident, state),
                AstType.Variable => ParseAssign(ident, state),
                AstType.Lambda => ParseLambda(ident, state),
                _ => throw new SyntaxError(ident.Token, $"Unhandled ast type ({ident.Type}) found."),
            };
            return ast;
        }

        public Ast_Base ParseIdentifier(Token token, ParserState state)
        {
            //Library lib;
            if (Keywords.Contains(token.Lexeme))
            {
                return new Ast_Keyword(token);
            }
            else if (BoolOperands.Contains(token.Lexeme))
            {
                return new Ast_BoolOp(token);
            }
            else if (token.Lexeme == Ast_Variable.NilValue)
            {
                return new Ast_Constant(token);
            }
            else if (state.PeekToken().Type == TokenType.OpAssignLambda)
            {
                return new Ast_Lambda(token);
            }
            else if (state.PeekToken().Type == TokenType.BracketLeft)
            {
                return new Ast_Call(token);
            }
            else if (VariableTypes.Contains(token.Lexeme))
            {
                return new Ast_Type(token);
            }
            else // assume variable.
            {
                Ast_Variable ast;
                if (token.Type == TokenType.Ref)
                {
                    token = Expect(TokenType.Identifier, state);
                    ast = new Ast_Variable(token)
                    {
                        Ref = true
                    };
                }
                else
                {
                    ast = new Ast_Variable(token);
                }
                var peek = state.PeekToken();
                if (peek.Type == TokenType.IndexLeft)
                {
                    ast.Index = ParseIndex(state);
                }
                return ast;
            }
        }

        public Ast_Base ParseAssign(Ast_Base ident, ParserState state)
        {
            var token = state.GetToken();
            var ast = new Ast_Assign(token)
            {
                Variable = ident as Ast_Variable,
                Operand = token,
                Expression = ParseExpression(TokenType.Semicolon, state)
            };
            if (!state.Scope.VariableExists(ast.Variable.Name))
            {
                state.Scope.Variables.Add(ast.Variable);
                if (ast.Expression?.Token?.Lexeme.ToString() == Ast_Variable.RecordValue)
                {
                    ast.Variable.DoSetValue(Ast_Variable.NewRecordValue);
                }
                else if (ast.Expression?.Token?.Lexeme.ToString() == Ast_Variable.ArrayValue)
                {
                    ast.Variable.DoSetValue(Ast_Variable.NewArrayValue);
                }
                else if (ast.Expression?.Token?.Lexeme.ToString() == Ast_Variable.ParamsValue)
                {
                    ast.Variable.DoSetValue(Ast_Variable.NewParamsValue);
                }
                else if (Constants.Contains(ast.Expression.Token.Type))
                {
                    ast.Variable.DoSetValue(ast.Expression.Token.Lexeme);
                }
            }
            else
            {
                var v = state.Scope.GetVariable(ast.Variable.Name);
                if ((v.Value.Type == ValueType.Array || v.Value.Type == ValueType.Record) && ast.Variable.Index == null)
                {
                    throw new SyntaxError(token, "Invalid value assignment to iterable variable.");
                }
            }
            _ = Expect(TokenType.Semicolon, state);
            return ast;
        }

        public Ast_Base ParseBlock(ParserState state)
        {
            var scope = state.Scope;
            state.Scope = scope.CreateChild("block");
            var ast = new Ast_Block(Expect(TokenType.BlockLeft, state));
            GetAst(ast, state);
            Expect(TokenType.BlockRight, state);
            state.Scope = scope;
            return ast;
        }

        public Ast_Scope ParseSubBlock(Ast_Base ast, ParserState state)
        {
            var scope = state.Scope;
            var child = scope.CreateChild("subblock");
            state.Scope = child;
            Expect(TokenType.BlockLeft, state);
            GetAst(ast, state);
            Expect(TokenType.BlockRight, state);
            state.Scope = scope;
            return child;
        }

        public Ast_Base ParseCall(Ast_Base ident, ParserState state)
        {
            if (ident?.Type != AstType.Call)
            {
                throw new SyntaxError(ident.Token, "Not a function or procedure call.");
            }

            _ = Expect(TokenType.BracketLeft, state);
            var ast = new Ast_Call(ident.Token);
            ParseParameters(ast, state);
            _ = Expect(TokenType.Semicolon, state);
            return ast;
        }

        private void ParseParameters(Ast_Base ast, ParserState state)
        {
            var terminators = new List<TokenType> { TokenType.BracketRight, TokenType.Comma };
            var token = state.PeekToken();
            while (token.Type != TokenType.BracketRight && token.Type != TokenType.EOF)
            {
                var expr = ParseExpression(terminators, state);
                if (expr != null)
                {
                    ast.Block.Add(expr);
                }
                token = state.PeekToken();
                if (token.Type == TokenType.Comma)
                {
                    _ = Expect(TokenType.Comma, state);
                }
            }
            _ = Expect(TokenType.BracketRight, state);
        }

        public Ast_Index ParseIndex(ParserState state)
        {
            var token = state.PeekToken();
            var index = new Ast_Index(token);
            while (token.Type == TokenType.IndexLeft)
            {
                Expect(TokenType.IndexLeft, state);
                token = state.PeekToken();
                if (token.Type == TokenType.IndexRight)
                {
                    var rt = Expect(TokenType.IndexRight, state);
                    token = state.PeekToken();
                    if (token.Type == TokenType.IndexLeft)
                    {
                        throw new SyntaxError(token, "Array: Invalid index after newindex");
                    }
                    index.Block.Add(new Ast_Expression(rt) { Type = AstType.NewArrayIndex });
                    break;
                }
                else
                {
                    index.Block.Add(ParseExpression(TokenType.IndexRight, state));
                }
                Expect(TokenType.IndexRight, state);
                token = state.PeekToken();
            }
            return index;
        }

        #region Dijkstra, Parsing an expression and convert to reverse Polish.
        private static bool OpPrecedence(KeyValuePair<int, string> left, KeyValuePair<int, string> right)
        {
            var prec1 = left.Key;
            var prec2 = right.Key;
            var assoc = left.Value;
            if ((assoc == "left" && prec2 >= prec1) || (assoc == "right" && prec2 > prec1))
            {
                return true;
            }
            return false;
        }

        public Ast_Expression ParseExpression(TokenType Terminator, ParserState state)
        {
            var term = new List<TokenType>
            {
                Terminator
            };
            return ParseExpression(term, state);
        }
        public Ast_Expression ParseExpression(List<TokenType> Terminator, ParserState state)
        {
            var token = state.PeekToken();
            var s = new Stack<Ast_Base>();
            Ast_Expression ast = new Ast_Expression(token);
            var level = 0;
            while (!state.EOF && !(level == 0 && token.Type == TokenType.IndexRight) && !Terminator.Contains(token.Type))
            {
                if ((token.Type == TokenType.Identifier) || (token.Type == TokenType.Ref))
                {
                    Ast_Base ident = ParseIdentifier(state.GetToken(), state);
                    if (ident.Type == AstType.Type)
                    {
                        ast.Block.Add(ident);
                        token = state.PeekToken();
                        continue;
                    }
                    else if (ident.Type == AstType.Constant && ident.Token.Lexeme == Ast_Variable.NilValue)
                    {
                        ast.Block.Add(ident);
                        token = state.PeekToken();
                        continue;
                    }
                    else if (ident.Type == AstType.BoolOp)
                    {
                        if (ident.Token.Lexeme == "not")
                        {
                            s.Push(ident);
                            token = state.PeekToken();
                            continue;
                        }
                        var op = ident;

                        while ((s.Count > 0) && (s.Peek().Token.Lexeme != "(") &&
                            OpPrecedence(precedenceTable[op.Token.Lexeme], precedenceTable[s.Peek().Token.Lexeme]))
                        {
                            var x = s.Pop();
                            ast.Block.Add(x);
                        }

                        s.Push(op);
                        token = state.PeekToken();
                        continue;
                    }
                    else if (ident.Type == AstType.Call)
                    {
                        ident = new Ast_Call(ident.Token);
                        _ = Expect(TokenType.BracketLeft, state);
                        ParseParameters(ident, state);
                    }
                    ast.Block.Add(ident);
                }
                else if (Constants.Contains(token.Type))
                {
                    Expect(token.Type, state);

                    ast.Block.Add(new Ast_Constant(token));
                }
                else if (token.Type == TokenType.BracketLeft)
                {
                    s.Push(new Ast_Constant(token) { Type = AstType.Bracket });
                    Expect(token.Type, state);
                    level += 1;
                }
                else if (token.Type == TokenType.BracketRight)
                {
                    Expect(TokenType.BracketRight, state);
                    level -= 1;
                    if (s.Count > 0)
                    {
                        var topToken = s.Pop();
                        while (topToken.Token.Type != TokenType.BracketLeft && s.Count > 0)
                        {
                            ast.Block.Add(topToken);
                            topToken = s.Pop();
                        }
                    }
                }
                else if (token.Type == TokenType.OpAssignLambda)
                {
                    token.Lexeme = "::_this_is_a_real_lambda_::";
                    var named = new Ast_Lambda(token);
                    var fn = ParseLambda(named, state);
                    ast.Block.Add(fn);
                }
                else if (Operands.Contains(token.Type))
                {
                    Expect(token.Type, state);
                    var op = new Ast_Op(token);
                    if (precedenceTable[op.Token.Lexeme].Value == "unary")
                    {
                        s.Push(op);
                        token = state.PeekToken();
                        continue;
                    }
                    while (s.Count > 0 && s.Peek().Token.Lexeme != "(" && OpPrecedence(precedenceTable[op.Token.Lexeme], precedenceTable[(s.Peek()).Token.Lexeme]))
                    {
                        var x = s.Pop();
                        ast.Block.Add(x);
                    }
                    s.Push(op);
                }
                else
                {
                    throw new SyntaxError(token, $"Unexpected token type ({token.Type}).");
                }
                token = state.PeekToken();
            }

            if (state.EOF)
            {
                throw new SyntaxError(token, $"Unexpected end of file.");
            }

            while (s.Count > 0)
            {
                var x = s.Pop();
                ast.Block.Add(x);
            }
            return ast;
        }

        #endregion

        public Ast_Base ParseKeyword(Ast_Base ident, ParserState state)
        {
            Ast_Base ast;
            if (ident.Token.Lexeme == "foreach")
            {
                ast = ParseForeach(ident, state);
            }
            else if (ident.Token.Lexeme == "break")
            {
                ast = ParseBreak(ident, state);
            }
            else if (ident.Token.Lexeme == "halt")
            {
                ast = ParseHalt(ident, state);
            }
            else if (ident.Token.Lexeme == "if")
            {
                ast = ParseIf(ident, state);
            }
            else if (ident.Token.Lexeme == "while")
            {
                ast = ParseWhile(ident, state);
            }
            else
            {
                throw new SyntaxError(ident.Token, $"Unexpected keyword {ident.Token.Lexeme}.");
            }
            return ast;
        }
        private Ast_Foreach ParseForeach(Ast_Base ident, ParserState state)
        {
            var ast = new Ast_Foreach(ident.Token);
            var token = state.GetToken();
            var id = ParseIdentifier(token, state);
            if (id.Type != AstType.Variable && id.Token.Type != TokenType.ConstantString)
            {
                throw new SyntaxError(token, $"Foreach requires a string or a variable.");
            }
            ast.Container = id;
            var child = ParseSubBlock(ast, state);
            child.Name = "foreach";
            var it = new Ast_Variable(new Token() { Lexeme = "it" });
            child.Variables.Add(it);
            return ast;
        }
        public Ast_Lambda ParseLambda(Ast_Base ident, ParserState state)
        {
            Ast_Lambda temp;
            if (ident is Ast_Lambda)
            {
                temp = ident as Ast_Lambda;
            }
            else
            {
                temp = new Ast_Lambda(ident.Token);
            }
            _ = Expect(TokenType.OpAssignLambda, state);
            _ = Expect(TokenType.BracketLeft, state);
            var token = state.PeekToken();
            while (token.Type != TokenType.BracketRight && token.Type != TokenType.EOF)
            {
                var id = ParseIdentifier(Expect(TokenType.Identifier, state), state);
                if (id?.Type != AstType.Variable)
                {
                    throw new SyntaxError(id?.Token, "Expected variable identifier as function/procedure argument.");
                }
                Ast_Variable arg = id as Ast_Variable;

                temp.Args.Add(arg);
                token = state.PeekToken();
                if (token.Type == TokenType.Comma)
                {
                    _ = Expect(TokenType.Comma, state);
                }
            }
            _ = Expect(TokenType.BracketRight, state);
            Ast_Lambda ast;
            token = state.PeekToken();
            if (token.Type == TokenType.OpAssignReturnVar)
            {
                Expect(TokenType.OpAssignReturnVar, state);
                ast = new Ast_Function(temp.Token)
                {
                    Args = temp.Args,
                    ReturnVariable = new Ast_Variable(Expect(TokenType.Identifier, state))
                };
            }
            else
            {
                ast = new Ast_Procedure(temp.Token)
                {
                    Args = temp.Args
                };
            }
            token = state.PeekToken();
            if (token.Type == TokenType.BlockLeft)
            {
                var sub = ParseSubBlock(ast, state);
                sub.Name = $"{ast.Type}";
                foreach (var arg in ast.Args)
                {
                    sub.Variables.Add(arg);
                }
            }
            else
            {
                Expect(TokenType.Semicolon, state);
            }
            return ast;
        }
        public static Ast_Break ParseBreak(Ast_Base ident, ParserState state)
        {
            var ast = new Ast_Break(ident.Token);
            _ = Expect(TokenType.Semicolon, state);
            return ast;
        }
        public Ast_Halt ParseHalt(Ast_Base ident, ParserState state)
        {
            var ast = new Ast_Halt(ident.Token)
            {
                Exitcode = ParseExpression(TokenType.Semicolon, state)
            };
            _ = Expect(TokenType.Semicolon, state);
            return ast;
        }
        private Ast_If ParseIf(Ast_Base ident, ParserState state)
        {
            var ast = new Ast_If(ident.Token)
            {
                Expression = ParseExpression(TokenType.BlockLeft, state)
            };
            var sub = ParseSubBlock(ast, state);
            sub.Name = "if";
            var token = state.PeekToken();
            if (token.Lexeme == "elif")
            {
                ast.ElIf = new Ast_ElIf(token);
            }
            while (state.PeekToken().Lexeme == "elif")
            {
                ParseElIf(ast, state);
            }
            if (state.PeekToken().Lexeme == "else")
                ParseElse(ast, state);
            return ast;
        }
        private void ParseElIf(Ast_If If, ParserState state)
        {
            var ast = new Ast_ElIf(Expect(TokenType.Identifier, state))
            {
                Expression = ParseExpression(TokenType.BlockLeft, state)
            };
            var sub = ParseSubBlock(ast, state);
            sub.Name = "elif";
            If.ElIf.Block.Add(ast);
        }
        private void ParseElse(Ast_If If, ParserState state)
        {
            var ast = new Ast_Else(Expect(TokenType.Identifier, state));
            var sub = ParseSubBlock(ast, state);
            sub.Name = "else";
            If.Else = ast;
        }
        private Ast_While ParseWhile(Ast_Base ident, ParserState state)
        {
            var ast = new Ast_While(ident.Token)
            {
                Expression = ParseExpression(TokenType.BlockLeft, state)
            };
            var sub = ParseSubBlock(ast, state);
            sub.Name = "while";
            return ast;
        }

    }
}
