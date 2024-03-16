using AutoBattle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattle
{
    public class BoardControl : MonoBehaviour
    {
        [SerializeField]
        private Transform boardRoot;
        [SerializeField]
        private TileModel tilePrefab;

        private List<List<TileModel>> tileContainer;

        int width;
        int height;
        public void InitBoard(int _width, int _height)
        {
            tileContainer = new List<List<TileModel>>();
            width = _width;
            height = _height;
            for (int i = 0; i < _width; i++)
            {
                tileContainer.Add(new List<TileModel>());
                for (int j = 0; j < _height; j++)
                {
                    TileModel tile = Instantiate(tilePrefab);
                    tile.transform.position = new Vector3(i, j, 0);
                    tile.transform.parent = boardRoot;
                    tile.Init(new Vector2(i, j));
                    tile.onClicked += OnCharacterSelected;
                    tileContainer[i].Add(tile);
                }
            }
        }

        CharacterControl currentChar;
        private void OnCharacterSelected(CharacterControl character)
        {
            if(currentChar != null)
            {
                currentChar.EnableHightlight(false);
            }
            currentChar = character;
            if (currentChar != null)
            {
                currentChar.EnableHightlight(true);
            }
        }

        public void SetCharacter(Vector2 pos, CharacterControl character, bool setPosition = false)
        {
            if (tileContainer != null
               && tileContainer.Count > pos.x
               && tileContainer[(int)pos.x].Count > pos.y
               && pos.x >= 0 && pos.y >= 0)
            {
                TileModel tileModel = tileContainer[(int)pos.x][(int)pos.y];
                tileModel.SetCharacter(character);
                character.SetPosition(tileModel, setPosition);
            }
        }

        public void MoveCharacter(List<Vector2> path, CharacterControl character)
        {
            int step = character.CharacterModel.MOVE;
            if (path != null && path.Count > 0 && step > 0)
            {
                List<Vector2> newPath = new List<Vector2>();
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    if(newPath.Count < step)
                    {
                        newPath.Add(path[i]);
                    }
                }

                SetCharacter(newPath[newPath.Count - 1], character);
                character.Move(newPath);
            }
        }

        public List<CharacterControl> GetCharacterAround(CharacterControl character, bool sameSide = true)
        {
            List<CharacterControl> neighbors = new List<CharacterControl> ();
            int range = character.CharacterModel.RANGE;

            for(int i = -1 * range; i <= range; i++)
            {
                for (int j = -1 * range; j <= range; j++)
                {
                    int newX = (int)character.CurrentTile.currentPos.x + i;
                    int newY = (int)character.CurrentTile.currentPos.y + j;

                    if (newX < 0
                        || newX >= width
                        || newY + j < 0
                        || newY + j >= height
                        || (i == 0 && j == 0))
                    {
                        continue;
                    }
                    else
                    {
                        CharacterControl tmpNeighbor = tileContainer[newX][newY].currentCharacter;
                        if (tmpNeighbor != null)                            
                        {
                            if(sameSide && tmpNeighbor.CurrentSide == character.CurrentSide)
                            {
                                neighbors.Add(tmpNeighbor);
                            }
                            if (!sameSide && tmpNeighbor.CurrentSide != character.CurrentSide)
                            {
                                neighbors.Add(tmpNeighbor);
                            }
                        }
                    }
                }
            }

            return neighbors;
        }

        #region Get shortest route

        private List<Vector2> GetAvailableNeighbor(Vector2 currentPos)
        {
            List<Vector2> neighborList = new List<Vector2>();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (currentPos.x + i < 0
                        || currentPos.x + i >= width
                        || currentPos.y + j < 0
                        || currentPos.y + j >= height
                        || (i == 0 && j == 0))
                    {
                        continue;
                    }
                    else
                    {
                        neighborList.Add(new Vector2(currentPos.x + i, currentPos.y + j));
                    }
                }
            }

            return neighborList;
        }

        public PathToEnemy DetectClosestEnemy(CharacterControl curChar)
        {
            if (curChar == null)
            {
                return null;
            }

            Vector2 currentPos = curChar.transform.position;
            PathToEnemy path = new PathToEnemy();
            Queue<PathComponent> neighborWaiting = new Queue<PathComponent>();
            PathComponent rootPath = new PathComponent()
            {
                curPos = currentPos,
                lastPos = null,
            };
            neighborWaiting.Enqueue(rootPath);

            HashSet<Vector2> passNeighbor = new HashSet<Vector2>();
            do
            {
                PathComponent tmpPos = neighborWaiting.Dequeue();
                passNeighbor.Add(tmpPos.curPos);
                List<Vector2> neighbors = GetAvailableNeighbor(tmpPos.curPos);

                if (neighbors.Count > 0)
                {
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        if (passNeighbor.Contains(neighbors[i]))
                            continue;

                        int neighborX = Mathf.RoundToInt(neighbors[i].x);
                        int neighborY = Mathf.RoundToInt(neighbors[i].y);
                        CharacterControl tmpNeibor = tileContainer[neighborX][neighborY].currentCharacter;
                        PathComponent neighborCpt = new PathComponent()
                        {
                            curPos = neighbors[i],
                            lastPos = tmpPos,
                        };

                        if (tmpNeibor == null )
                        {
                            if (tileContainer[neighborX][neighborY].currentCharacter == null)
                            {
                                neighborWaiting.Enqueue(neighborCpt);
                            }                            
                        }
                        else if (curChar.CurrentSide != tmpNeibor.CurrentSide)
                        {
                            path.curPos = currentPos;
                            path.enemyPos = neighbors[i];
                            path.path = GetRoute(neighborCpt);
                            return path;
                        }
                    }
                }
            } while (neighborWaiting.Count > 0);


            return null;
        }

        public List<Vector2> GetRoute(PathComponent startPos)
        {
            List<Vector2> route = new List<Vector2>();
            if (startPos.lastPos == null)
            {
                Debug.Log("No need to move " + startPos.curPos);
            }
            else
            {
                PathComponent tmpPath = startPos.lastPos;
                do
                {
                    route.Add(tmpPath.curPos);
                    if(tmpPath.lastPos != null)
                        tmpPath = tmpPath.lastPos;
                }
                while (tmpPath.lastPos != null);

            }
            return route;
        }
        #endregion
    }

    public class PathComponent
    {
        public Vector2 curPos;
        public PathComponent lastPos;
    }
    public class PathToEnemy
    {
        public List<Vector2> path;
        public Vector2 enemyPos;
        public Vector2 curPos;

        public override string ToString()
        {
            string s = "From: " + curPos.ToString()
                + " To: " + enemyPos.ToString()
                + "| Path: ";
            if (path != null && path.Count > 0)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    s = s + path[i].ToString() + " -> ";
                }
                s += "end";
            }
            else
            {
                s = s + "null";
            }
            return s;
        }
    }

}
