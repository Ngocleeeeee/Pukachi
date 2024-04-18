﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.WallMode
{
    // Changed Object: Node: đổi adj, Matrix: đổi val
    // Constant Object: Sprites
    public class Base1
    {
        public static int[,] data;
        public static int[,] MATRIX;
        public static int m, n;
        public static Sprite[] lstSprites; // cố định
        public static Transform gridParent;
        public static Dictionary<int, int> FREQUENCY = new Dictionary<int, int>()
        {
            {1, 6 },
            {2, 6 },
            {3, 6},
            {4, 6},
            {5, 6},
            {6, 6},
            {7, 6},
            {8, 6},
            {9, 6},
            {10,6 },
            {11, 6},
            {12, 6}
        };
   
        public void GenerateMatrix(int m, int n)
        {
            Base1.m = m;
            Base1.n = n;
            //Around
            MATRIX = new int[m + 2,n + 2];
            for (int i = 0; i < m + 2; i++)
            {
                for (int j = 0; j < n + 2; j++)
                {
                    MATRIX[i,j] = 0;
                }
            }

            MATRIX[0, 0] = -1;
            MATRIX[0, n + 1] = -1;
            MATRIX[m + 1, n + 1] = -1;
            MATRIX[m + 1, 0] = -1;
            //Inside
            RandomMatrix(m, n);
            //LogMatrix(MATRIX);
            RenderMatrix(m, n);
        }

        public virtual void RenderMatrix(int m, int n)
        {
            GameObject gridParentObject = GameObject.FindWithTag("Grid");

            var brickPrefab = GameObject.FindWithTag("firstOBJ");
            var brickHelp = GameObject.FindWithTag("zeroOBJ");
            
            if (gridParentObject != null)
            {
                gridParent = gridParentObject.transform;
            }
            Sprite firstSprite = lstSprites[0];
            var brickWidth = firstSprite.bounds.size.x;
            var brickHeight = firstSprite.bounds.size.y;

            // Lấy vị trí của Sprite đầu tiên
            var firstSpriteX = brickPrefab.transform.position.x;
            var firstSpriteY = brickPrefab.transform.position.y;
            //LogMatrix(MATRIX);

            for (int i = 0; i < m+2; i++)
            {
                for (int j = 0; j < n+2; j++)
                {
                    var val = MATRIX[i, j] ;
                    GameObject brick;
                    if (val == 0 && i != 0 && j != 0 && i != m + 1 && j != n + 1) continue;
                    if (val == 0 || val == -1)
                    {
                        brick = GameObject.Instantiate(brickHelp, gridParent);
                    }
                    else
                    {
                        Sprite selectedSprite = lstSprites[val];
                        brick = GameObject.Instantiate(brickPrefab, gridParent);
                        SpriteRenderer spriteRenderer = brick.GetComponent<SpriteRenderer>();
                        spriteRenderer.sprite = selectedSprite;
                    }

                    float xPosition = firstSpriteX + (j * brickWidth);
                    float yPosition = firstSpriteY - (i * brickHeight);
                    brick.transform.position = new Vector3(xPosition, yPosition, 0);
                    //brick.tag = val.ToString();
                    brick.name = i + "," + j;
                    

                }
            }
        }

        private void RandomMatrix(int m, int n)
        {
            System.Random random = new System.Random();

            //Lấy theo tần suất lưu vào 1 list
            List<int> numbers = new List<int>();
            foreach (var map in FREQUENCY)
            {
                var number = map.Key;
                var freq = map.Value;
                for (int i = 0; i < freq; i++)
                {
                    numbers.Add(number);
                }
            }

            // Trộn ngẫu nhiên danh sách số
            for (int i = 0; i < numbers.Count; i++)
            {
                int temp = numbers[i];
                int randomIndex = random.Next(i, numbers.Count);
                numbers[i] = numbers[randomIndex];
                numbers[randomIndex] = temp;
            }
            

            // Đặt các số từ danh sách vào vị trí ngẫu nhiên trong ma trận
            int index = 0;
            for (int i = 1; i < m+1; i++)
            {
                for (int j = 1; j < n+1; j++)
                {
                    MATRIX[i, j] = numbers[index];
                    index++;
                }
            }
            //Gravity
            //data = MATRIX;
            
        }

        public virtual void ResetMatrix()
        {
            GameObject gridParentObject = GameObject.FindWithTag("Grid");

            // Kiểm tra nếu đối tượng cha tồn tại
            if (gridParentObject != null)
            {
                // Lấy danh sách các đối tượng con
                Transform parentTransform = gridParentObject.transform;
                int childCount = parentTransform.childCount;

                // Xóa các đối tượng con không phải là hai đối tượng đầu tiên
                for (int i = 2; i < childCount; i++)
                {
                    if (parentTransform.GetChild(i).gameObject.name.Contains("!")) continue;
                    GameObject.Destroy(parentTransform.GetChild(i).gameObject);
                }
            }

            RandomMatrix(Base1.m, Base1.n);
            
            RenderMatrix(Base1.m, Base1.n);
        }

        public static bool IsCellInMaxtrix(Cell1 cell)
        {
            int m = Base1.MATRIX.GetLength(0);
            int n = Base1.MATRIX.GetLength(1);
            if (cell.i < 0 || cell.i >=m || cell.j <0 || cell.j >= n)
            {
                Debug.Log("Cell1 out of MATRIX");
                return false;
            }
            if (MATRIX[cell.i, cell.j] == 0)
            {
                Debug.Log("Cell1 NULL");
                return false;
            }

            return true;
        }

        public static Cell1 GetCell(string name)
        {
            if (!name.Contains(",")) return null;
            var cellInfor = name.Split(',');
            int i, j;
            int.TryParse(cellInfor[0],out i);
            int.TryParse(cellInfor[1],out j);
            if (i == 0 || j == 0 || i == Base1.m + 1 || j == Base1.n + 1) return null;
            return new Cell1(i, j);
           
        }

        public static void LogMatrix(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            for (int row = 0; row < rows; row++)
            {
                string rowString = string.Empty;

                for (int column = 0; column < columns; column++)
                {
                    rowString += matrix[row, column].ToString() + " ";
                }

                Debug.Log(rowString.Trim());
            }
        }

       

      
    }


}