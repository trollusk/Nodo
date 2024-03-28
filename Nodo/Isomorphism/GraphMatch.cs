using Nodo.Extensions;

namespace Nodo.Isomorphism;


public class GraphMatch<TVertex> : IEnumerable<Tuple<TVertex,TVertex>> 
    where TEdge : IEdge<TVertex>
    where TGraph : IGraph<TVertex, TEdge>
{
    private Dictionary<int, int> _match;
    private TGraph _world;
    private TGraph _motif;

    // Ctor
    public GraphMatch (Dictionary<int, int> initDict, TGraph world, TGraph motif)
    {
        _match = new Dictionary<int, int>(initDict);    // copy of initDict
        _world = world;
        _motif = motif;
    }

    /// Returns a tuple (w, m) where w is the world vertex and m is the matched motif vertex.
    public IEnumerator GetEnumerator()
    {
        foreach ((int worldIndex, int motifIndex) in _match)
        {
            yield return (_world.Vertices[worldIndex], _motif.Vertices[motifIndex]);
        }
    }

    public TVertex GetMatchedVertex(TVertex vertex)
    {
        int worldIndex = _world.Vertices.FindIndex(v => Object.ReferenceEquals(vertex, v));
        if (worldIndex >= 0)
        {
            return _motif.Vertices[worldIndex];
        }
        int motifIndex = _motif.Vertices.FindIndex(v => Object.ReferenceEquals(vertex, v));
        if (motifIndex >= 0)
        {
            return _world.Vertices[motifIndex];
        }
        throw InvalidOperationException("Neither graph contains the vertex: {vertex}");
        return null;
    }

    // Return the edge in the "other" graph, that matches this one.
    public TEdge GetMatchedEdge(TEdge edge)
    {
        TVertex matchedSource = GetMatchedVertex(edge.Source);
        TVertex matchedTarget = GetMatchedVertex(edge.Target);
        // Find Edge in other graph, that has (matchedSource -> matchedTarget)

        TGraph graph = _world.Edges.Find(edge) ? _world : _motif;
        TEdge matchedEdge = graph.Edges.Find(e => e.Source.ReferenceEquals(matchedSource) && e.Target.ReferenceEquals(matchedTarget));
        return matchedEdge;
    }

    /// Returns the subgraph of WORLD which has been matched to MOTIF, as a new graph.
    public TGraph GetSubgraph()
    {
        List<TVertex> worldVertices = _match.Keys.Select(index => _world.Vertices[index]);
        List<TEdge> worldEdges = _world.Edges.Where(edge => worldVertices.Find(edge.Source) && worldVertices.Find(edge.Target));

        TGraph sub = new(worldVertices, worldEdges);
        return sub;
    }
}

// foreach (match in matcher.Match())
// {
//      (match bound to a GraphMatch)
// }