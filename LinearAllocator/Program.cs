using System;
using System.Collections.Generic;
using System.Text;

namespace Allocator
{
    class Program
    {
        unsafe static void Main()
        {
            LinearAllocator.Initialize();
            int* a = (int*)LinearAllocator.MemAlloc(12);
            *a = 9999;
            int* b = (int*)LinearAllocator.MemAlloc(4);
            *b = 1234;
            a = (int*)LinearAllocator.MemReAlloc(a, 4);
            int* c = (int*)LinearAllocator.MemAlloc(4);
            *c = 7777;
            LinearAllocator.MemFree(b);
            LinearAllocator.Dump();
        }
    }
}
