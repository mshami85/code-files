using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Compression
{
    internal class HuffmanAlgorithm
    {
        public static string Compress(string data, out Dictionary<char, string> codeTable)
        {
            var tree = new Tree(data);
            var table = tree.GetCodeTable();
            codeTable = table;
            return string.Join("", data.Select(c => table[c]));
        }

        public string Decompress(string data, Dictionary<char, string> codeTable)
        {
            throw new NotImplementedException();
        }



        class Tree
        {
            private readonly List<Node> _nodes;
            private readonly Node _root;

            public Tree(string data)
            {
                _nodes = Init(data);
                _root = Build() ?? new(0);
            }
            private static List<Node> Init(string data)
            {
                Dictionary<char, int> frequencyTable = new();
                foreach (char chr in data)
                {
                    if (!frequencyTable.ContainsKey(chr))
                    {
                        frequencyTable.Add(chr, 0);
                    }
                    frequencyTable[chr]++;
                }
                return frequencyTable.Select(skv => new Node(skv.Key, skv.Value)).ToList();
            }
            private Node? Build()
            {
                while (_nodes.Count > 1)
                {
                    _nodes.Sort();
                    var taken = _nodes.Take(2).ToList();
                    var parent = new Node(taken[0].Frequency + taken[1].Frequency)
                    {
                        Left = taken[0],
                        Right = taken[1],
                    };
                    parent.Left.Parent = parent;
                    parent.Right.Parent = parent;

                    _nodes.Remove(taken[0]);
                    _nodes.Remove(taken[1]);
                    _nodes.Add(parent);
                }
                return _nodes.FirstOrDefault();
            }
            public Dictionary<char, string> GetCodeTable()
            {
                Dictionary<char, string> codes = new();
                _root.Traverse(node =>
                {
                    if (node.Symbol != Node.SpecialChar)
                    {
                        codes.Add(node.Symbol, node.GetCode());
                    }
                });
                return codes;
            }

            class Node : IComparable<Node>
            {
                public const char SpecialChar = '\u001D';
                public int Frequency { get; }
                public char Symbol { get; }
                public Node? Left { get; set; }
                public Node? Right { get; set; }
                public Node? Parent { get; set; }

                public string GetCode()
                {
                    if (Parent == null)
                    {
                        return string.Empty;
                    }
                    return Parent.GetCode() + (Parent.Right == this ? "1" : "0");
                }
                public Node(char symbol, int freq)
                {
                    Frequency = freq;
                    Symbol = symbol;
                }
                public Node(int frerq)
                {
                    Frequency = frerq;
                    Symbol = SpecialChar;
                }
                public int CompareTo(Node? other)
                {
                    return Frequency.CompareTo(other?.Frequency);
                }
                public void Traverse(Action<Node> action)
                {
                    action(this);
                    Left?.Traverse(action);
                    Right?.Traverse(action);
                }
            }
        }
    }


}
