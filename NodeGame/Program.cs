using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeGame
{
    class Node
    {
        public Node(Graph graph, string name)
        {
            Name = name;
            Graph = graph;
            Graph.Nodes[Name] = this;
        }

        public string Name { get; set; }
        public Graph Graph { get; set; }
        public IEnumerable<Edge> Edges
        {
            get
            {
                return Graph.Edges.Where(e => e.Node1 == this || e.Node2 == this);
            }
        }
        public IEnumerable<Node> Neighbours
        {
            get
            {
                return Graph.Edges.Where(e => e.Node1 == this).Select(e => e.Node2)
                    .Concat(Graph.Edges.Where(e => e.Node2 == this).Select(e => e.Node1));
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class Edge
    {
        public Edge(Graph graph, string nodes)
        {
            Graph = graph;
            if (Graph.Nodes.ContainsKey(nodes[0].ToString()))
                Node1 = Graph.Nodes[nodes[0].ToString()];
            else
                Node1 = Graph.Nodes[nodes[0].ToString()] = new Node(Graph, nodes[0].ToString());
            if (Graph.Nodes.ContainsKey(nodes[1].ToString()))
                Node2 = Graph.Nodes[nodes[1].ToString()];
            else
                Node2 = Graph.Nodes[nodes[1].ToString()] = new Node(Graph, nodes[1].ToString());

            Graph.Edges.Add(this);
        }

        public Graph Graph { get; set; }
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }

        public override string ToString()
        {
            return Node1.Name + Node2.Name;
        }
    }

    class Graph
    {
        public Graph()
        {
            Edges = new List<NodeGame.Edge>();
            Nodes = new Dictionary<string, NodeGame.Node>();
        }

        public Graph(string spec) : this()
        {
            foreach (string edgeSpec in spec.Split(' '))
            {
                new Edge(this, edgeSpec);
            }
        }

        public List<Edge> Edges { get; set; }
        public Dictionary<string, Node> Nodes { get; set; }
        public bool IsEmpty
        {
            get
            {
                return !Nodes.Any();
            }
        }

        public void Remove(Node node)
        {
            Edges.RemoveAll(e => node.Edges.ToList().Contains(e));
            Nodes.Remove(node.Name);
        }

        public Graph Clone()
        {
            var clone = new Graph();
            Nodes.Values.Select(n => new Node(clone, n.Name)).ToList();
            Edges.Select(e => new Edge(clone, e.Node1.Name + e.Node2.Name)).ToList();
            return clone;
        }
    }

    class Program
    {
        static bool Play(Graph graph, string choice)
        {
            var node = graph.Nodes[choice];
            foreach (var neighbour in node.Neighbours.ToList())
            {
                graph.Remove(neighbour);
            }
            graph.Remove(node);

            return graph.IsEmpty;
        }

        static Node FindWin(Graph graph)
        {
            var options = graph.Nodes.Keys.ToDictionary(n => n, n => graph.Clone());
            var best = options.Keys.FirstOrDefault(n => Play(options[n], n));
            if (best == null)
            {
                best = options.Keys.FirstOrDefault(n => FindWin(options[n]) == null);
            }
            if (best != null)
            {
                return graph.Nodes[best];
            }
            return null;
        }

        static void Main(string[] args)
        {
            var graph = new Graph("AB AC AD BD BE CD CF CG CJ DE DF DG DI EI EM FG GH GI GJ HI HJ HL IL IM IQ JK JL JO JP JQ JS KN KP KR LQ MQ MU NP NR OS PR PS PT PU QS QU RT SU TU");
            Play(graph, "L");
            Play(graph, "S");
            Play(graph, "C");
            var winningMove = FindWin(graph);
        }
    }
}
