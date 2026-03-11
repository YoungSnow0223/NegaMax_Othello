# NegaMax_Othello

[AI Name]
Othello.AI.NegaMax

[File Location]
dll is placed under Othello.AI.NegaMax > bin > Debug > net8.0

[How my AI works]
This AI uses the NegaMax algorithm with Alpha-Beta pruning to select the best moves in Othello.  
It evaluates board positions using a weighted approach based on Othello strategy guidelines:  
    - Corners are the most valuable positions.  
    - Edges are valuable but less than corners.  
    - Squares adjacent to corners are dangerous.  
    - Early in the game, the AI focuses on positional advantage; later, it focuses on maximizing the number of discs.  

Official strategy reference: "How to play Othello"(https://www.eothello.com/#how-to-play)

[Core Components]
1. BestMove
    Finds the optimal move from all valid moves by simulating each move and scoring the resulting board using NegaMax. 
    
2. NegaMax
   Searches repeatedly to a defined depth, alternating turns between the AI and the opponent.  
   Uses Alpha-Beta pruning to skip branches that cannot lead to better outcome, improving search efficiency.
   
3. EvaluateBoard
   Calculates a numerical score for the board from the AI's perspective.  
   - Assigns weights to each cell based on strategic importance according to Othello strategy (corners, edges, and dangerous squares near corners).  
   - Considers positional advantage early in the game and disc count in the late game to maximize the final score.  




