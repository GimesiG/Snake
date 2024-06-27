using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int rows { get; }
        public int columns { get; }

        public GridValue[,] grid { get; }
        public Direction dir { get; private set; }
        public int score { get; private set; }
        public bool gameOver { get; private set; }

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            grid = new GridValue[rows, columns];
            dir = Direction.right;

            addSnake();
            addFood();
        }

        private void addSnake()
        {
            int r = rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> emptyPositions()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void addFood()
        {
            List<Position> empty = new List<Position>(emptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            grid[pos.column, pos.column] = GridValue.Food;
        }

        public Position headPosition()
        {
            return snakePositions.First.Value;
        }

        public Position tailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> snakePosition()
        {
            return snakePositions;
        }

        private void addHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            grid[pos.row, pos.column] = GridValue.Snake;
        }

        private void removeTail()
        {
            Position tail = snakePositions.Last.Value;
            grid[tail.row, tail.column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        private Direction getLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return dir;
            }
            return dirChanges.Last.Value;
        }

        private bool canChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }

            Direction lastDir = getLastDirection();

            return newDir != lastDir && newDir != lastDir.opposite();
        }

        public void changeDirection(Direction dir)
        {
            if (canChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }

        private bool outsideGrid(Position pos)
        {
            return pos.row < 0 || pos.row >= rows || pos.column < 0 || pos.column >= columns;
        }

        private GridValue willHit(Position newHeadPos)
        {
            if (outsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }

            if (newHeadPos == tailPosition())
            {
                return GridValue.Empty;
            }

            return grid[newHeadPos.row, newHeadPos.column];
        }

        public void move()
        {
            if (dirChanges.Count > 0)
            {
                dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            Position newHeadPos = headPosition().translate(dir);
            GridValue hit = willHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                gameOver = true;
            }

            else if (hit == GridValue.Empty)
            {
                removeTail();
                addHead(newHeadPos);
            }

            else if (hit == GridValue.Food)
            {
                addHead(newHeadPos);
                score++;
                addFood();
            }
        }



    }
}
