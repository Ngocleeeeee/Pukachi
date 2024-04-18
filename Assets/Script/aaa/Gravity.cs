using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Script.WallMode
{
    public class Gravity : Base1
    {
        public void ApplyGravity(bool fromBottomToTop)
        {
            int startFromRow = fromBottomToTop ? 1 : MATRIX.GetLength(0) - 2;
            int step = fromBottomToTop ? 1 : -1;
            int toEndRow = fromBottomToTop ? MATRIX.GetLength(0) - 1 : 0;

            for (int column = 1; column < MATRIX.GetLength(1) - 1; column++)
            {
                int newRow = startFromRow; 
                int rowIncrement = step; 

                for (int row = startFromRow; row != toEndRow; row += rowIncrement)
                {
                    if (MATRIX[row, column] == 0)
                    {

                            for (int nextRow = row + rowIncrement; newRow != toEndRow; nextRow += rowIncrement)
                            {
                                // Nếu gặp ô không trống
                                if (MATRIX[nextRow, column] != 0 && MATRIX[nextRow, column] != -1)
                                {
                                    // Đẩy giá trị của ô không trống xuống hoặc lên ô trống hiện tại
                                    MATRIX[newRow, column] = MATRIX[nextRow, column];
                                    // Đặt giá trị của ô không trống bằng 0
                                    MATRIX[nextRow, column] = 0;
                                    newRow += rowIncrement; // Di chuyển tới hàng tiếp theo
                                }
                                else
                                {
                                    break; // Dừng lại nếu gặp ô trống
                                }
                            }
                            
                        
                    }
                    else
                    {
                        // Nếu ô hiện tại không phải là ô trống, di chuyển tới hàng tiếp theo hoặc hàng trước đó
                        newRow += rowIncrement;
                    }
                }
            }

            //LogMatrix(MATRIX);
            RenderMatrix(m, n);
        }
        public override void RenderMatrix(int m, int n)
        {
            ResetMatrix();

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

            for (int i = 0; i < m + 2; i++)
            {
                for (int j = 0; j < n + 2; j++)
                {
                    var val = MATRIX[i, j];
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

        public override void ResetMatrix()
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
        }

    }
}