namespace Wanderers
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			using (var game = new WanderersGame())
			{
				game.Run();
			}
		}
	}
}