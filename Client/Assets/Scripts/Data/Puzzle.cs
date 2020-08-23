using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName="puzzle.asset", menuName="Puzzle")]
public class Puzzle : ScriptableObject
{
	public Sprite[] allPics = { };

	public void MakePuzzleItem(VisualElement addTo)
	{
		foreach(var piece in allPics)
		{
			Image curr = new Image();
			curr.image = piece.texture;
			curr.style.flexGrow = 1;
			curr.style.height = addTo.style.height;
			addTo.Add(curr);
		}
	}
}
