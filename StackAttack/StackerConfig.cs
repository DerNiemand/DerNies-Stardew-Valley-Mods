using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackAttack
{
	public sealed class StackerConfig
	{
		public float gridScale = 1;

		public int cellSize = 32;
		public int innerBorderThickness = 4;
		public int outerBorderThickness = 8;

		public int columnCount = 7;
		public int rowCount = 11;

		public int smallRewardRow = 7;
		public int largeRewardRow = 11;


		public double timeBetweenMoves = 1.0;
		public Dictionary<int,double> moveTimeModifiers = new Dictionary<int, double>();



        public List<string> smallRewardList = new List<string>();
		public List<string> largeRewardList = new List<string>();

    }
}
