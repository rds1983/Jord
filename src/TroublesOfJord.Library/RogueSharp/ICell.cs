using System;

namespace RogueSharp
{
	/// <summary>
	/// A class that defines a square on a Map with all of its associated properties
	/// </summary>
	public interface ICell : IEquatable<ICell>
	{
		/// <summary>
		/// Gets the X location of the Cell starting with 0 as the farthest left
		/// </summary>
		int X { get; set; }

		/// <summary>
		/// Y location of the Cell starting with 0 as the top
		/// </summary>
		int Y { get; set; }

		ICellInfo Info { get; set; }

		/// <summary>
		/// Check if the Cell is in the currently computed field-of-view
		/// For newly initialized maps a field-of-view will not exist so all Cells will return false
		/// Field-of-view must first be calculated by calling ComputeFov and/or AppendFov
		/// </summary>
		/// <remarks>
		/// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius
		/// </remarks>
		/// <example>
		/// Field-of-view can be used to simulate a character holding a light source and exploring a Map representing a dark cavern
		/// Any Cells within the FOV would be what the character could see from their current location and lighting conditions
		/// </example>
		bool IsInFov { get; set; }

		/// <summary>
		/// Check if the Cell is flagged as ever having been explored by the player
		/// </summary>
		/// <remarks>
		/// The explored property of a Cell can be used to track if the Cell has ever been in the field-of-view of a character controlled by the player
		/// This property will not automatically be updated based on FOV calculations or any other built-in functions of the RogueSharp library.
		/// </remarks>
		/// <example>
		/// As the player moves characters around a Map, Cells will enter and exit the currently computed field-of-view
		/// This property can be used to keep track of those Cells that have been "seen" and could be used to show fog-of-war type effects when rendering the map
		/// </example>
		bool IsExplored { get; set; }
	}
}