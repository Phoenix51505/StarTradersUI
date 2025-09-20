using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace StarTradersUI.Utilities;

public class QuadTree<T>(double minX, double maxX, double minY, double maxY, uint depth) : IEnumerable<T>
    where T : class, IPositionable
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
        private List<T>? _boundedItems;

        public double MidX => (MinX + MaxX) / 2;
        public double MidY => (MinY + MaxY) / 2;


        public void Add(T item, uint remainingDepth)
        {
            if (!Bounds(item)) throw new ArgumentException("QuadTreeNode does not bound item", nameof(item));
            if (remainingDepth == 0)
            {
                lock (this)
                {
                    _boundedItems ??= [];
                    _boundedItems.Add(item);
                }
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
            if (minX >= MaxX) yield break;
            if (minY >= MaxY) yield break;
            if (maxX < MinX) yield break;
            if (maxY < MinY) yield break;

            if (_boundedItems != null)
            {
                lock (this)
                {
                    foreach (var item in _boundedItems.Where(x =>
                                 x.X.IsBoundedBy(minX, maxX) && x.Y.IsBoundedBy(minY, maxY)))
                    {
                        yield return item;
                    }
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
            lock (this)
            {
                _boundedItems?.Remove(item);
                if (_boundedItems?.Count == 0) _boundedItems = null;
            }

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

        public double ClosestPossibleDistanceTo(double x, double y)
        {
            var xBounded = x.IsBoundedBy(MinX, MaxX);
            var yBounded = y.IsBoundedBy(MinY, MaxY);
            var closestPossibleX = xBounded ? x : x < MinX ? MinX : MaxX;
            var closestPossibleY = yBounded ? y : y < MinY ? MinY : MaxY;
            return SquareDistance(x, y, closestPossibleX, closestPossibleY);
        }

        public T? ClosestTo(double x, double y, double preComparedDistance = double.MaxValue)
        {
            // If we know that this node cannot possibly have a closer match 
            if (ClosestPossibleDistanceTo(x, y) >= preComparedDistance)
            {
                return null;
            }

            if (_boundedItems != null)
            {
                // Bounded items must always contain an item so we can use first
                lock (this)
                {
                    return _boundedItems.OrderBy(i => SquareDistance(x, y, i.X, i.Y))
                        .FirstOrDefault(i => SquareDistance(x, y, i.X, i.Y) < preComparedDistance);
                }
            }

            List<QuadTreeNode> children = [];
            if (TopLeft != null)
            {
                children.Add(TopLeft);
            }

            if (TopRight != null)
            {
                children.Add(TopRight);
            }

            if (BottomLeft != null)
            {
                children.Add(BottomLeft);
            }

            if (BottomRight != null)
            {
                children.Add(BottomRight);
            }

            T? current = null;

            foreach (var node in children.OrderBy(n => n.ClosestPossibleDistanceTo(x, y)))
            {
                var currentCheck = node.ClosestTo(x, y, preComparedDistance);
                if (currentCheck == null) continue;
                preComparedDistance = SquareDistance(currentCheck.X, currentCheck.Y, x, y);
                current = currentCheck;
            }

            return current;
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

    public T? ClosestTo(double x, double y) => Root.ClosestTo(x, y);

    #region Utilities

    private static double SquareDistance(double x1, double y1, double x2, double y2)
    {
        return (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
    }

    #endregion
}