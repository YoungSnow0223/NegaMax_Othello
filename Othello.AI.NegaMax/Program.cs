using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Othello.Contract;
using Othello.Engine;

namespace Othello.AI.NegaMax;

public class NegaMaxAI : IOthelloAI
{
    public string Name => "NegaMax AI";

    private const int SearchDepth = 6;
    
    public async Task<Move?> GetMoveAsync(BoardState board, DiscColor yourColor, CancellationToken ct)
    {
        await Task.Delay(new System.Random().Next(100, 1000), ct);

        var validMoves = GetValidMoves(board, yourColor);
        if (validMoves.Count == 0) return null;
        
        return BestMove(validMoves, board, yourColor);
    }
    
    private Move? BestMove(List<Move> validMoves, BoardState board, DiscColor color)
    {
        int bestScore = int.MinValue;
        Move? bestMove = null;

        foreach (var move in validMoves)
        {
            var newBoard = board.Clone();
            
            GameLogic.ApplyMove(newBoard, move, color);

            int score = -NegaMax(newBoard, Opponent(color), color, SearchDepth-1, int.MinValue, int.MaxValue);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }    
        }
        return bestMove;
    }
    
    private int NegaMax(BoardState board, DiscColor color, DiscColor yourColor, int depth, int alpha, int beta)
    {
        int bestScore = int.MinValue;
        var moves = GetValidMoves(board, color);

        if (depth == 0)
        {
            return EvaluateBoard(board, yourColor);
        }
        
        if (moves.Count == 0) 
        {
           var oppMoves = GetValidMoves(board, Opponent(color));
           if (oppMoves.Count == 0)
           {
               return EvaluateBoard(board, yourColor);
           }
           return -NegaMax(board, Opponent(color), yourColor, depth - 1, -beta, -alpha);
        }
        
        foreach (var move in moves)
        {
            var newBoard = board.Clone();
            
            GameLogic.ApplyMove(newBoard, move, color);
            
            int score = -NegaMax(newBoard, Opponent(color), yourColor, depth - 1, -beta, -alpha);

            if (score > bestScore)
            {
                bestScore = score;
            }
            
            if (bestScore > alpha)
            {
                alpha = bestScore;
            }

            if (alpha >= beta)
            {
                break;
            }
            
        }
        return bestScore;
    }

    private int EvaluateBoard(BoardState board, DiscColor color)
    {
        int score = 0;
        int myDisc = 0;
        int oppDisc = 0;
        
        int[,] weights = new int[8, 8]
        {
            { 100, -25, 10,  5,  5, 10, -25, 100 },
            { -25, -50, -2, -2, -2, -2, -50, -25 },
            {  10,  -2,  0,  0,  0,  0,  -2,  10 },
            {   5,  -2,  0,  0,  0,  0,  -2,   5 },
            {   5,  -2,  0,  0,  0,  0,  -2,   5 },
            {  10,  -2,  0,  0,  0,  0,  -2,  10 },
            { -25, -50, -2, -2, -2, -2, -50, -25 },
            { 100, -25, 10,  5,  5, 10, -25, 100 }
        };

        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (board.Grid[r, c] == color)
                {
                    score += weights[r, c];
                    myDisc++;
                }
                else if (board.Grid[r, c] == Opponent(color))
                {
                    score -= weights[r, c];
                    oppDisc++;
                }
            }
        }
        
        int total = myDisc + oppDisc;
        if (total > 55)
        {
            score = myDisc - oppDisc;
        }
        
        return score;
    }

    private DiscColor Opponent(DiscColor color)
    {
        if (color == DiscColor.Black)
        {
            return DiscColor.White;
        }
        else
        {
            return DiscColor.Black;
        }
    }
    
    //All available move
    private List<Move> GetValidMoves(BoardState board, DiscColor color)
    {
        var moves = new List<Move>();
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (IsValidMove(board, new Move(r, c), color))
                {
                    moves.Add(new Move(r, c));
                }
            }
        }
        return moves;
    }
    
    //Determine if move is valid
    private bool IsValidMove(BoardState board, Move move, DiscColor color)
    {
        if (board.Grid[move.Row, move.Column] != DiscColor.None) return false;
        
        int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };
        DiscColor opponent = color == DiscColor.Black ? DiscColor.White : DiscColor.Black;

        for (int i = 0; i < 8; i++)
        {
            int r = move.Row + dr[i];
            int c = move.Column + dc[i];
            int count = 0;

            while (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == opponent)
            {
                r += dr[i];
                c += dc[i];
                count++;
            }

            if (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == color && count > 0)
            {
                return true;
            }
        }
        return false;
    }
}