
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField]
    private DialogueNode[] nodes;

    public DialogueNode[] Nodes { get => nodes; set => nodes = value; }
}
