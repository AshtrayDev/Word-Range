using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;

public class KDNode
{
    public string Word { get; }
    public float[] Embedding { get; }
    public KDNode Left { get; set; }
    public KDNode Right { get; set; }

    public KDNode(string word, float[] embedding)
    {
        Word = word;
        Embedding = embedding;
    }
}

public class KDTree : MonoBehaviour
{
    private KDNode root;

    public void BuildTree(Dictionary<string, float[]> embeddings)
    {
        if (embeddings == null || embeddings.Count == 0)
        {
            // Handle empty embeddings dictionary or null reference
            return;
        }

        root = BuildKDTree(embeddings, 0);
    }

    private KDNode BuildKDTree(Dictionary<string, float[]> embeddings, int depth)
    {
        if (embeddings.Count == 0)
            return null;

        int axis = depth % embeddings.First().Value.Length;
        var sortedWords = embeddings.OrderBy(kv => kv.Value[axis]).ToList();
        int medianIndex = sortedWords.Count / 2;

        return new KDNode(sortedWords[medianIndex].Key, sortedWords[medianIndex].Value)
        {
            Left = BuildKDTree(sortedWords.Take(medianIndex).ToDictionary(kv => kv.Key, kv => kv.Value), depth + 1),
            Right = BuildKDTree(sortedWords.Skip(medianIndex + 1).ToDictionary(kv => kv.Key, kv => kv.Value), depth + 1)
        };
    }

	public Dictionary<string, float[]> FindNearestNeighbors(string targetWord, int numNeighbors, Dictionary<string, float[]> embeddings)
	{
		PriorityQueue<KeyValuePair<string, float>> neighborsQueue = new PriorityQueue<KeyValuePair<string, float>>(Comparer<KeyValuePair<string, float>>.Create((a, b) => -a.Value.CompareTo(b.Value)));

		FindNearestNeighbors(root, targetWord, neighborsQueue, numNeighbors, embeddings, 0);

		Dictionary<string, float[]> nearestNeighbors = new Dictionary<string, float[]>();
		while (neighborsQueue.Count > 0)
		{
			nearestNeighbors.Add(neighborsQueue.Dequeue().Key, embeddings[neighborsQueue.Dequeue().Key]);
		}
		return nearestNeighbors;
	}

	private void FindNearestNeighbors(KDNode currentNode, string targetWord, PriorityQueue<KeyValuePair<string, float>> neighborsQueue, int numNeighbors, Dictionary<string, float[]> embeddings, int depth)
	{
		if (currentNode == null)
			return;

		float distance = CalculateDistance(currentNode.Embedding, embeddings[targetWord]);
		neighborsQueue.Enqueue(new KeyValuePair<string, float>(currentNode.Word, distance));

		if (neighborsQueue.Count > numNeighbors)
			neighborsQueue.Dequeue();

		int axis = depth % currentNode.Embedding.Length;
		float axisDist = embeddings[targetWord][axis] - currentNode.Embedding[axis];

		KDNode nearerSubtree = axisDist < 0 ? currentNode.Left : currentNode.Right;
		KDNode fartherSubtree = axisDist < 0 ? currentNode.Right : currentNode.Left;

		FindNearestNeighbors(nearerSubtree, targetWord, neighborsQueue, numNeighbors, embeddings, depth + 1);

		if (neighborsQueue.Count < numNeighbors || Math.Abs(axisDist) < neighborsQueue.Peek().Value)
			FindNearestNeighbors(fartherSubtree, targetWord, neighborsQueue, numNeighbors, embeddings, depth + 1);
	}


    private float CalculateDistance(float[] embedding1, float[] embedding2)
    {
        float sum = 0;
        for (int i = 0; i < embedding1.Length; i++)
            sum += (embedding1[i] - embedding2[i]) * (embedding1[i] - embedding2[i]);
        return (float)Math.Sqrt(sum);
    }
}

public class PriorityQueue<T>
{
    private List<T> data;
    private readonly IComparer<T> comparer;

    public PriorityQueue(IComparer<T> comparer)
    {
        this.data = new List<T>();
        this.comparer = comparer;
    }

    public void Enqueue(T item)
    {
        data.Add(item);
        int childIndex = data.Count - 1;
        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (comparer.Compare(data[childIndex], data[parentIndex]) >= 0)
                break;
            T tmp = data[childIndex];
            data[childIndex] = data[parentIndex];
            data[parentIndex] = tmp;
            childIndex = parentIndex;
        }
    }

    public T Dequeue()
    {
        if (data.Count == 0)
            throw new InvalidOperationException("Queue is empty");
        int lastIndex = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[lastIndex];
        data.RemoveAt(lastIndex);

        lastIndex--;
        int parentIndex = 0;
        while (true)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > lastIndex)
                break;
            int rightChildIndex = leftChildIndex + 1;
            if (rightChildIndex <= lastIndex && comparer.Compare(data[rightChildIndex], data[leftChildIndex]) < 0)
                leftChildIndex = rightChildIndex;
            if (comparer.Compare(data[parentIndex], data[leftChildIndex]) <= 0)
                break;
            T tmp = data[parentIndex];
            data[parentIndex] = data[leftChildIndex];
            data[leftChildIndex] = tmp;
            parentIndex = leftChildIndex;
        }
        return frontItem;
    }

    public T Peek()
    {
        if (data.Count == 0)
            throw new InvalidOperationException("Queue is empty");
        return data[0];
    }

    public int Count
    {
        get { return data.Count; }
    }
}
