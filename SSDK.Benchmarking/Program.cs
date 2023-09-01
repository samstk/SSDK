// See https://aka.ms/new-console-template for more information

using SSDK.Benchmarking;
using SSDK.Core.Helpers;
using SSDK.Core.Algorithms.Sorting;

Console.WriteLine("--SSDK Benchmarking--");
Console.WriteLine("Time values may be inaccurate, as it depends on the state of the machine.");
Console.WriteLine("However, they will provide a guide as to faster algorithms in general.");

Console.WriteLine("**Sorting Algorithms");
Console.WriteLine("All sorting algorithms are based on 10000 elements.");

int[] array = null;
Benchmarker.Do(() =>
{
    array = NumberGenerator.CreateRandomIntegers(10000);
}, "Array (10000) Generation: ");

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

// Perform binary search on sorted list
Random rg = new Random();
int rgi = rg.Next(0, array2.Length);
int el = array2[rgi];
Benchmarker.Do(() =>
{
    el = array2.BinarySearch(el);
}, "Binary Search: ");
Console.WriteLine("(Index) = " + el);
