// See https://aka.ms/new-console-template for more information

using SSDK.Benchmarking;
using SSDK.Core.Helpers;
using SSDK.Core.Algorithms.Sorting;
using SSDK.Core.Structures.Trees;
using System;
using KBS.Core.Arithmetic;
using SSDK.Core.Structures.Graphs;

int INTEGER_COUNT = 10000;

Console.WriteLine("--SSDK Benchmarking--");
Console.WriteLine("Time values may be inaccurate, as it depends on the state of the machine.");
Console.WriteLine("However, they will provide a guide as to faster algorithms in general.");

Console.WriteLine("**Graph Algorithms");
Graph<int> graph = new Graph<int>();
GraphVertex<int> g1 = graph.Add(1),
    g2 = graph.Add(2),
    g3 = graph.Add(3),
    g4 = graph.Add(4),
    g5 = graph.Add(5);

graph.CreatePath(g1, g3, 3);
graph.CreatePath(g2, g4, 4);
graph.Join(g4, g1, 2);
graph.Join(g4, g5, 2);
graph.Join(g5, g3, 2);

Benchmarker.Do(() =>
{
    GraphTraversal<int> traversal = graph.DepthFirstSearch(g2);
}, "DFS Traversal on 1: ");

Console.WriteLine("**Sorting Algorithms");
Console.WriteLine($"All sorting algorithms are based on {INTEGER_COUNT} elements.");
int[] array = null;
Benchmarker.Do(() =>
{
    array = NumberGenerator.CreateRandomIntegers(INTEGER_COUNT);
}, "Array Generation: ");

// Perform selection sort
int[] array2 = array.DeepClone();
Benchmarker.Do(() =>
{
    array2.SortViaSelection();
}, "Selection Sort: ");


// Perform insertion sort
array2 = array.DeepClone();
Benchmarker.Do(() =>
{
    array2.SortViaInsertion();
}, "Insertion Sort: ");
array2 = array.DeepClone();

// Perform merge sort
array2 = array.DeepClone();
Benchmarker.Do(() =>
{
    array2.SortViaMerge();
}, "Merge Sort: ");

// Perform quick sort
array2 = array.DeepClone();
Benchmarker.Do(() =>
{
    array2.SortViaQuickSort();
}, "Quick Sort: ");
if (!array2.IsSorted()) Console.WriteLine("ERR");

// Perform bucket sort

array2 = array.DeepClone();
Benchmarker.Do(() =>
{
    array2.SortViaBucket(3, (val) =>
    {
        if (val == 0) return 1;
        else if (val < 0) return 0;
        return 2;
    });
}, "Bucket Sort (<0, 0, >0): ");

// Perform binary search on sorted list
Random rg = new Random();
int rgi = rg.Next(0, array2.Length);
int el = array2[rgi];
Benchmarker.Do(() =>
{
    el = array2.BinarySearch(el);
}, "Binary Search: ");
Console.WriteLine("(Index) = " + el);

HeapTree<int> heapTest = new HeapTree<int>();

Benchmarker.Do(() => {
    foreach(int element in array)
    {
        heapTest.Add(element);
    }
}, "Adding elements to heap: ");

PriorityQueue<int, int> queue = new PriorityQueue<int, int>();
Benchmarker.Do(() => {
    foreach (int element in array)
    {
        queue.Enqueue(element, element);
    }
}, "Adding elements to native priority queue: ");
HeapTree<int> heapTest2 = heapTest.Clone(true);

Benchmarker.Do(() => {
    foreach (int element in array)
    {
        if(heapTest.Remove(element) == null)
        {
            Console.Write("ERR ");
            break;
        }
       
    }
}, "Removing random elements from heap: ");



Benchmarker.Do(() => {
    int i = 0;
    foreach (int element in array)
    {
        if (heapTest2.Remove(element) == null)
        {
            Console.Write("ERR at "+i+" ");
            break;
        }
        i++;

    }
}, "Removing min-elements from heap: ");

Benchmarker.Do(() => {
    foreach (int element in array)
    {
        queue.Dequeue();
    }
}, "Removing elements from native priority queue: ");

heapTest = new HeapTree<int>();

Benchmarker.Do(() =>
{
    for(int i = 0; i<5000; i++)
    {
        for(int x = 0; x<2; x++)
        {
            heapTest.Add(i * 2 + x);
        }
        heapTest.Remove(i * 2);
    }
}, "Sequence of adding (10000) and removals (5000) to heap: ");

BinarySearchTree<int> bst = new BinarySearchTree<int>();
Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length; i++)
    {
        bst.Add(array[i]);
    }
}, "Adding elements to BST: ");

Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length; i++)
    {
        bst.Remove(array[i]);
    }
}, "Removing elements from BST: ");

SplaySearchTree<int> splay = new SplaySearchTree<int>();
Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length; i++)
    {
        splay.Add(array[i]);
        if(splay.RootNode.Value != array[i])
        {

        }
    }
}, "Adding elements to Splay Tree: ");

Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length; i++)
    {
        splay.Remove(array[i]);
    }
}, "Removing elements from Splay Tree: ");

AVLSearchTree<int> avl = new AVLSearchTree<int>();

Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length; i++)
    {
        avl.Add(array[i]);
    }
}, "Adding elements to AVL: ");

Benchmarker.Do(() =>
{
    int errors = 0;
    foreach(var node in avl)
    {
        if(!node.IsBalanced)
        {
            errors++;
        }
    }
    Console.WriteLine($"[{errors} errors found]");
}, "Checking AVL Balance Properties: ");

Benchmarker.Do(() =>
{
    for (int i = 0; i < array.Length / 2; i++)
    {
        avl.Remove(array[i]);
    }
}, "Removing half-elements from AVL: ");

Benchmarker.Do(() =>
{
    int errors = 0;
    foreach (var node in avl)
    {
        if (!node.IsBalanced)
        {
            errors++;
        }
    }
    Console.WriteLine($"[{errors} errors found]");
}, "Checking AVL Balance Properties: ");

Benchmarker.Do(() =>
{
    for (int i = array.Length / 2; i < array.Length; i++)
    {
        avl.Remove(array[i]);
    }
}, "Removing rest of elements from AVL: ");

Console.WriteLine("Finished");