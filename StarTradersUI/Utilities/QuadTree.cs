using System;
using System.Collections;
using System.Collections.Generic;

namespace StarTradersUI.Utilities;

public class QuadTree<T>(double minX, double maxX, double minY, double maxY, uint depth) : IEnumerable<T>
    where T : IPositionable
{
    public class QuadTreeNode(double minX, double minY, double maxX, double maxY) : IEnumerable<T>
    {
        public QuadTreeNode? TopLeft;
        public QuadTreeNode? TopRight;
        public QuadTreeNode? BottomLeft;
        public QuadTreeNode? BottomRight;
        public double MinX = minX; // Inclusive
        public double MinY = minY; // Inclusive
        public double MaxX = maxX; // Exclusive
        public double MaxY = maxY; // Exclusive
        public List<T>? BoundedItems;

        public double MidX => (MinX + MaxX) / 2;
        public double MidY => (MinY + MaxY) / 2;


        public void Add(T item, uint remainingDepth)
        {
            if (!Bounds(item)) throw new ArgumentException("QuadTreeNode does not bound item", nameof(item));
            if (remainingDepth == 0)
            {
                BoundedItems ??= [];
                BoundedItems.Add(item);
            }
            else
            {
                var mx = MidX;
                var my = MidY;
                if (item.X < mx && item.Y < my)
                {
                    BottomLeft ??= new QuadTreeNode(MinX, MinY, mx, my);
                    BottomLeft.Add(item, remainingDepth - 1);
                }
                else if (item.X < mx && item.Y >= my)
                {
                    TopLeft ??= new QuadTreeNode(MinX, my, mx, MaxY);
                    TopLeft.Add(item, remainingDepth - 1);
                }
                else if (item.X >= mx && item.Y < my)
                {
                    BottomRight ??= new QuadTreeNode(mx, MinY, MaxX, my);
                    BottomRight.Add(item, remainingDepth - 1);
                }
                else if (item.X >= mx && item.Y >= my)
                {
                    TopRight ??= new QuadTreeNode(mx, my, MaxX, MaxY);
                    TopRight.Add(item, remainingDepth - 1);
                }
            }
        }

        public IEnumerator<T> SearchInBounds(double minX, double minY, double maxX, double maxY)
        {
            // First check if the bounds being searched actually contain any of the points
            if (minX >= maxX) yield break;
            if (minY >= maxY) yield break;
            if (maxX < minX) yield break;
            if (maxY < minY) yield break;

            if (BoundedItems != null)
            {
                foreach (var item in BoundedItems)
                {
                    yield return item;
                }
            }

            if (TopLeft != null)
            {
                var subEnumerator = TopLeft.SearchInBounds(minX, minY, maxX, maxY);
                while (subEnumerator.MoveNext())
                {
                    yield return subEnumerator.Current;
                }
            }

            if (TopRight != null)
            {
                var subEnumerator = TopRight.SearchInBounds(minX, minY, maxX, maxY);
                while (subEnumerator.MoveNext())
                {
                    yield return subEnumerator.Current;
                }
            }

            if (BottomLeft != null)
            {
                var subEnumerator = BottomLeft.SearchInBounds(minX, minY, maxX, maxY);
                while (subEnumerator.MoveNext())
                {
                    yield return subEnumerator.Current;
                }
            }

            if (BottomRight != null)
            {
                var subEnumerator = BottomRight.SearchInBounds(minX, minY, maxX, maxY);
                while (subEnumerator.MoveNext())
                {
                    yield return subEnumerator.Current;
                }
            }
        }

        public bool Bounds(T item) => item.X.IsBoundedBy(MinX, MaxX) && item.Y.IsBoundedBy(MinY, MaxY);

        public void Remove(T item)
        {
            if (!Bounds(item)) return;
            BoundedItems?.Remove(item);
            TopLeft?.Remove(item);
            TopRight?.Remove(item);
            BottomLeft?.Remove(item);
            BottomRight?.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() => SearchInBounds(MinX, MinY, MaxX, MaxY);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public readonly QuadTreeNode Root = new(minX, minY, maxX, maxY);
    public readonly uint Depth = depth;

    public IEnumerator<T> GetEnumerator() => Root.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        Root.Add(item, Depth);
    }

    public void Remove(T item)
    {
        Root.Remove(item);
    }

    public IEnumerable<T> SearchInBounds(double minX, double minY, double maxX, double maxY)
    {
        var iterator = Root.SearchInBounds(minX, minY, maxX, maxY);
        while (iterator.MoveNext())
        {
            yield return iterator.Current;
        }
    }
}