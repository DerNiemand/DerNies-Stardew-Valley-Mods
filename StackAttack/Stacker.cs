using ExtensionMethods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Minigames;
using System.Diagnostics;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewModdingAPI;

namespace StackAttack
{
	internal class Stacker : IMinigame
	{
        
        private Random rand;

		private StackerConfig data;

		private float windowScale = 1f;
		
		private int screenwidth;
		private int screenheight;

		private bool screenMidPointDirty = true;
		private Vector2 cachedScreenMidPoint;
		private Vector2 ScreenMidPoint
		{
			get
			{
				if (screenMidPointDirty)
				{
					cachedScreenMidPoint = new Vector2(screenwidth / 2, screenheight / 2);
				}

				return cachedScreenMidPoint;
			}
		}


		private static Texture2D BG;

        private static Texture2D Grid;

        private bool gridPosDirty = true;
		private Vector2 cachedGridPos;
		private Vector2 GridPosition
		{
			get
			{
				if (gridPosDirty)
				{
					cachedGridPos = ScreenMidPoint - (Grid.HalfBounds() * data.gridScale * windowScale);
				}
				
				return cachedGridPos;
			}
		}

		private ClickableTextureComponent placeButton;
        private readonly int placeButtonWidth;
		private readonly int placeButtonHeight;

		private ClickableTextureComponent cashoutButton;
        private readonly int cashoutButtonWidth;
        private readonly int cashoutButtonHeight;

        private Texture2D? activeText;
		private static Texture2D RewardNoneText;
		private static Texture2D RewardSmallText;
		private static Texture2D RewardLargeText;

        private List<string> SmallRewardList
		{
			get
			{
				return data.smallRewardList;
			}
		}

        private List<string> LargeRewardList
		{
			get
			{
				return data.largeRewardList;
			}
		}


        private bool textPosDirty = true;
		private Vector2 cachedTextPos;
		private Vector2 TextPosition
		{
			get
			{
				if (textPosDirty)
				{
					if (activeText == null)
					{
						return Vector2.Zero;
					}

					cachedTextPos = new Vector2(ScreenMidPoint.X,ScreenMidPoint.Y * 0.1f) - (activeText.HalfBounds() * windowScale);

					textPosDirty = false;
				}

				return cachedTextPos;

			}
		}

		private List<BlockLine> blockLines;
		private BlockLine? nextLine;
		private BlockLine? prevLine;

		private double timeToNextMove;
		private double moveTimeModifier;

		private bool paused;
		private bool gameOver;
		private bool finished;

		public Stacker()
		{
			rand = new Random();

			screenwidth = Game1.graphics.GraphicsDevice.Viewport.Bounds.Width;
			screenheight = Game1.graphics.GraphicsDevice.Viewport.Bounds.Height;

            float newXScale = screenwidth / 1920f;
            float newYScale = screenheight / 1080f;

            windowScale = Math.Min(newXScale, newYScale);

            BG = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/BG.png");

			Grid = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/Grid.png");

			RewardNoneText = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/GameOverScreen/RewardTextNone.png");
			RewardSmallText = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/GameOverScreen/RewardTextSmall.png");
			RewardLargeText = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/GameOverScreen/RewardTextLarge.png");

			data = ModEntry.modHelper.ModContent.Load<StackerConfig>("assets/StackerConfig.json");


			var placeButtonTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/Buttons/PlaceButton");
			placeButtonHeight = placeButtonTexture.Height;
			placeButtonWidth = placeButtonTexture.Width;
            placeButton = new ClickableTextureComponent(new Rectangle((int)ScreenMidPoint.X - placeButtonWidth/2, (int)ScreenMidPoint.Y - (placeButtonHeight/2) + (int)(0.45 * screenheight), placeButtonWidth, placeButtonHeight),
														placeButtonTexture ,Rectangle.Empty,1);

            var cashoutButtonTexture = ModEntry.modHelper.ModContent.Load<Texture2D>("assets/Buttons/CashOutButton");
            cashoutButtonHeight = cashoutButtonTexture.Height;
            cashoutButtonWidth = cashoutButtonTexture.Width;
            cashoutButton = new ClickableTextureComponent(new Rectangle((int)ScreenMidPoint.X - (cashoutButtonWidth / 2) + (int)(0.2 * ScreenMidPoint.X), (int)ScreenMidPoint.Y - (cashoutButtonHeight / 2) + (int)(0.45 * screenheight), cashoutButtonWidth, cashoutButtonHeight),
                                                        cashoutButtonTexture, Rectangle.Empty, 1);
			cashoutButton.visible = false;


            blockLines = new List<BlockLine>();

			timeToNextMove = data.timeBetweenMoves;

            ModEntry.modHelper.Events.Display.WindowResized += OnScreenSizeChanged;

            finished = false;
		}


		public static bool Start()
		{
			Game1.currentMinigame = (IMinigame) new Stacker();

			if (Game1.currentMinigame is not  Stacker)
			{
				return false;
			}

            return true;
		}

		public void End()
		{
			finished = true;
			Game1.currentMinigame = null;
		}

		public void changeScreenSize()
		{
			
		}

		public bool doMainGameUpdates()
		{
			return false;
		}

		public void draw(SpriteBatch b)
		{
			b.Begin();
			b.Draw(BG, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.White);
			b.Draw(Grid, GridPosition, null, Color.White, 0, Vector2.Zero, data.gridScale * windowScale, SpriteEffects.None, 0);

			foreach (var line in blockLines)
			{
				line.Draw(b);
			}

			if (activeText != null)
			{
				b.Draw(activeText, TextPosition, null, Color.White, 0, Vector2.Zero, windowScale, SpriteEffects.None, 0);
			}

			placeButton.draw(b);
			cashoutButton.draw(b);

			b.End();
		}

		public bool forceQuit()
		{
			throw new NotImplementedException();
		}

		public void leftClickHeld(int x, int y)
		{
		}

		public string minigameId()
		{
			return "Stack Attack";
		}

		public void OnCashoutButtonPressed()
		{
			paused = false;
			gameOver = true;
			HandleRewardLevel();
		}

		public void OnPlaceButtonPressed()
		{
			if (paused)
			{
				paused = false;
				cashoutButton.visible = false;
			}
			else if(!gameOver)
			{
                StopLine();
            }
			
		}

		public void OnScreenSizeChanged(object? sender, WindowResizedEventArgs Args)
		{
			screenwidth = Args.NewSize.X;
            screenheight = Args.NewSize.Y;

            float newXScale = screenwidth / 1920f;
			float newYScale = screenheight / 1080f;

			windowScale = Math.Min(newXScale, newYScale);


            Console.WriteLine("ScreenSizeChanged!");


			screenMidPointDirty = true;
			gridPosDirty = true;
			textPosDirty = true;


			Vector2 placeButtonPos = new Vector2((screenwidth - placeButtonWidth)/2, (screenheight * 0.95f) - (placeButtonHeight/2) );
			placeButton.setPosition(placeButtonPos);

            Vector2 cashoutButtonPos = new Vector2((screenwidth * 0.7f) - (placeButtonWidth / 2), (screenheight * 0.95f) - (placeButtonHeight / 2));
            cashoutButton.setPosition(cashoutButtonPos);

            for (int i = 0; i < blockLines.Count; i++)
			{
				var line = blockLines[i];
				float[] rowPos = new float[line.BlockCount];
				for (int j = 0;j < line.BlockCount; j++)
				{
					rowPos[j] = (GetCollumnPixelLocation(line.CurrentColumn - j));
				}
				line.UpdateScale(data.gridScale * windowScale, rowPos, GetRowPixelLocation(i));
            }
				

		}

		public bool overrideFreeMouseMovement()
		{
			return true;
		}

		public void receiveEventPoke(int data)
		{
		}

		public void receiveKeyPress(Keys k)
		{
			if (k == Keys.Escape)
			{
				End();
			}

			if (k == Keys.Space)
			{
				OnPlaceButtonPressed();
			}
		}

		public void receiveKeyRelease(Keys k)
		{
		}

		public void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (placeButton.containsPoint(x, y))
			{
				OnPlaceButtonPressed();
			}
			else if(cashoutButton.containsPoint(x, y))
			{
				if (cashoutButton.visible)
				{
					OnCashoutButtonPressed();
				}
			}
		}

		public void receiveRightClick(int x, int y, bool playSound = true)
		{
		}

		public void releaseLeftClick(int x, int y)
		{
		}

		public void releaseRightClick(int x, int y)
		{
		}

		private void StopLine()
		{
			if (prevLine != null && nextLine != null)
			{
				var targetColumns = prevLine.GetBlockColumns();
				var blocksLeft = nextLine.CheckBlockColumns(targetColumns);
				if (!blocksLeft)
				{
					blockLines.Remove(nextLine);

					gameOver = true;

					HandleRewardLevel();
				}
			}

			prevLine = nextLine;
			nextLine = null;

			if (blockLines.Count == data.rowCount)
			{
				gameOver = true;

				HandleRewardLevel();
			}
			else if(blockLines.Count == data.smallRewardRow)
			{
				paused = true;
				cashoutButton.visible = true;
			}

			if(data.moveTimeModifiers.TryGetValue(blockLines.Count, out var modifier))
			{
				moveTimeModifier += modifier;
			}

			timeToNextMove = 0;
		}

		public bool tick(GameTime time)
		{
			double deltaTime = time.ElapsedGameTime.TotalSeconds;

			if (!paused)
			{
				timeToNextMove -= deltaTime;

				if (!gameOver)
				{

					if (timeToNextMove <= 0)
					{
						timeToNextMove = data.timeBetweenMoves - moveTimeModifier;

						if (nextLine == null)
						{
							if (blockLines.Count == 0)
							{
								nextLine = new BlockLine(3, data.gridScale * windowScale, data.columnCount);
							}
							else
							{
								nextLine = new BlockLine(prevLine.GetBlockCount(), data.gridScale * windowScale, data.columnCount);
							}

							blockLines.Add(nextLine);

							nextLine.StartLine(new Vector2(GetCollumnPixelLocation(0), GetRowPixelLocation(blockLines.Count - 1)));
						}
						else
						{
							nextLine.Move(new Vector2((data.cellSize + data.innerBorderThickness) * data.gridScale * windowScale, 0));

						}
					}
				}
			}

			return finished;
		}

        private void HandleRewardLevel()
        {
            if (blockLines.Count == data.largeRewardRow)
            {
                activeText = RewardLargeText;
				if (data.largeRewardList.Count > 0)
				{
					Item item = ItemRegistry.Create(LargeRewardList[rand.Next(LargeRewardList.Count)]);
					Game1.player.addItemToInventory(item);
				}
            }
            else if (blockLines.Count == data.smallRewardRow)
            {
                activeText = RewardSmallText;
				if(data.smallRewardList.Count > 0)
				{
					Item item = ItemRegistry.Create(SmallRewardList[rand.Next(SmallRewardList.Count)]);
					Game1.player.addItemToInventory(item);
				}
            }
            else
            {
                activeText = RewardNoneText;
            }
        }

        public void unload()
		{
		}

		private float GetRowPixelLocation(int row)
		{
			float gridBottomPixel = GridPosition.Y + (Grid.Height * data.gridScale * windowScale);
			return gridBottomPixel - ((data.outerBorderThickness + data.cellSize) * data.gridScale * windowScale) - ((data.innerBorderThickness + data.cellSize) * data.gridScale * windowScale * row);
		}

		private float GetCollumnPixelLocation(int col)
		{
			float gridLeftPixel = GridPosition.X;
			return gridLeftPixel + (data.outerBorderThickness * data.gridScale * windowScale) + ((data.innerBorderThickness + data.cellSize) * data.gridScale * windowScale * col);
		}
	}
}
