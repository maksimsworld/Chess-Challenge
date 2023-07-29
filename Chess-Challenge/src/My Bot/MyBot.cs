using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move best = GetBestMove(board, timer.MillisecondsRemaining);
        return best;
    }

    // Bitboard-based evaluation function
    static int EvaluateBoard(Board board)
    {
        int score = 0;

        for (int pieceType = 1; pieceType < 7; pieceType++)
        {
            ulong whiteBitboard = board.GetPieceBitboard((PieceType)pieceType, true);
            ulong blackBitboard = board.GetPieceBitboard((PieceType)pieceType, false);

            int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
            score += pieceValues[pieceType] * (BitboardHelper.GetNumberOfSetBits(whiteBitboard) - BitboardHelper.GetNumberOfSetBits(blackBitboard));
        }

        return score;
    }
// shitty recursive minimax
    static (int, Move) Minimax(Board board, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw()) {
            return (EvaluateBoard(board), Move.NullMove);
        }


        if (maximizingPlayer)
        {
            int maxEval = int.MinValue;
            Move bestMove = board.GetLegalMoves()[0];
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                var eval = Minimax(board, depth - 1, alpha, beta, !maximizingPlayer);
                board.UndoMove(move);
                if (eval.Item1 > maxEval)
                {
                    maxEval = eval.Item1;
                    bestMove = move;
                }
                alpha = Math.Max(alpha, eval.Item1);
                if (beta <= alpha)
                    break;
            }
            return (maxEval, bestMove);
        }
        else
        {
            int minEval = int.MaxValue;
            Move bestMove = board.GetLegalMoves()[0];
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                var eval = Minimax(board, depth - 1, alpha, beta, !maximizingPlayer);
                board.UndoMove(move);
                if (eval.Item1 < minEval)
                {
                    minEval = eval.Item1;
                    bestMove = move;
                }
                beta = Math.Min(beta, eval.Item1);
                if (beta <= alpha)
                    break;
            }
            return (minEval, bestMove);
        }
    }

    // Function to get the best move for the given position
    public static Move GetBestMove(Board board, int remainingTime)
    {

        // Set a fixed depth for simplicity (you can adjust this based on your needs)
        int depth = 5;
        // Get the best move using minimax with alpha-beta pruning
        var (_, bestMove) = Minimax(board, depth, int.MinValue, int.MaxValue, board.IsWhiteToMove);

        return bestMove;
    }

}

