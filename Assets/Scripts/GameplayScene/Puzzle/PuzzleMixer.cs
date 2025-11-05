using UnityEngine;

namespace MyGame.Gameplay.Puzzle
{
    public sealed class PuzzleMixer
    {
        public void MixParts(PuzzlePart[,] parts, Vector2Int partsLength)
        {
            for (int y = 0; y < partsLength.y; y++)
            {
                for (int x = 0; x < partsLength.x; x++)
                {
                    int randomX = Random.Range(0, partsLength.x);
                    int randomY = Random.Range(0, partsLength.y);
                    (parts[x, y], parts[randomX, randomY]) = (parts[randomX, randomY], parts[x, y]);
                }
            }
        }
    }
}