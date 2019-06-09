using Myra.Graphics2D.UI;
using Wanderers.Core;
using Wanderers.MapEditor.UI;
using IItemWithId = Wanderers.Core.IItemWithId;

namespace Wanderers.MapEditor.UI
{
	public class Explorer: Pane
	{
		private Module _project;

		public Module Project
		{
			get
			{
				return _project;
			}

			set
			{
				if (value == _project)
				{
					return;
				}

				_project = value;

				Rebuild();
			}
		}

		public Tree Tree
		{
			get { return (Tree) Widget; }
		}

		public Explorer()
		{
			Widget = new Tree();
		}

		public TreeNode AddItemToTree(IItemWithId item, TreeNode root)
		{
			var node = root.AddSubNode(item.Id);
			item.IdChanged += (sender, args) =>
			{
				node.Text = item.Id;
			};

			node.Tag = item;

			return node;
		}

		private void Rebuild()
		{
			var root = Tree;
			Tree.RemoveAllSubNodes();

			if (_project == null)
			{
				return;
			}

			root.Text = "Root";

			/*TileInfosNode = root.AddSubNode("TileInfos");

			foreach (var tileInfo in _project.TileInfos.Values)
			{
				AddItemToTree(tileInfo, TileInfosNode);
			}

			MapsNode = root.AddSubNode("Maps");
			foreach (var map in _project.Maps.Values)
			{
				AddItemToTree(map, MapsNode);
			}

			CharactersNode = root.AddSubNode("Characters");
			foreach (var character in _project.Characters.Values)
			{
				AddItemToTree(character, CharactersNode);
			}

			PartiesNode = root.AddSubNode("Parties");
			foreach (var party in _project.Parties.Values)
			{
				var partyNode = AddItemToTree(party, PartiesNode);

				foreach (var partyMember in party.Members)
				{
					AddItemToTree(partyMember, partyNode);
				}
			}*/
		}
	}
}
