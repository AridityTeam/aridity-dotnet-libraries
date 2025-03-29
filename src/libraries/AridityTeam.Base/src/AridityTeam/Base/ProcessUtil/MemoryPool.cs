﻿/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace AridityTeam.Base.ProcessUtil
{
    public class MemoryPool
    {
        private readonly ConcurrentBag<IntPtr> _freeBlocks;
        private readonly int _initialBlockCount;

        public int BlockSize { get; }

        public MemoryPool(int blockSize, int initialBlockCount)
        {
            BlockSize = blockSize;
            _initialBlockCount = initialBlockCount;
            _freeBlocks = [];

            // Pre-allocate memory blocks
            for (var i = 0; i < _initialBlockCount; i++)
            {
                _freeBlocks.Add(Marshal.AllocHGlobal(BlockSize));
            }
        }

        public IntPtr Rent()
        {
            if (_freeBlocks.TryTake(out var block))
            {
                return block; // Return a free block
            }

            // If no free blocks are available, allocate a new one
            return Marshal.AllocHGlobal(BlockSize);
        }

        public void Return(IntPtr block)
        {
            if (block != IntPtr.Zero)
            {
                _freeBlocks.Add(block); // Return the block to the pool
            }
        }

        public void Clear()
        {
            while (_freeBlocks.TryTake(out var block))
            {
                Marshal.FreeHGlobal(block); // Free all blocks in the pool
            }
        }
    }
}
