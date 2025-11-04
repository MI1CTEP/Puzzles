using UnityEngine;

namespace MyGame
{
    public static class PuzzleMixer
    {
        public static void MixParts(PuzzlePart[,] parts, Vector2Int partsLength, int openLinesY)
        {
            for (int i = 0; i <= partsLength.y - openLinesY; i++)
            {
                for (int y = 0; y < openLinesY; y++)
                {
                    for (int x = 0; x < partsLength.x; x++)
                    {
                        int randomX = Random.Range(0, partsLength.x);
                        int randomY = Random.Range(0, openLinesY);
                        (parts[x, y + i], parts[randomX, randomY + i]) = (parts[randomX, randomY + i], parts[x, y + i]);
                    }
                }
            }
        }
    }
}