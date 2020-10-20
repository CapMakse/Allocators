﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace Allocator
{
    unsafe class LinearAllocator
    {
        static LinearAllocator Instance;
        byte[] Memory;

        private LinearAllocator()
        {
            Memory = new byte[1024];
            fixed (byte* byt = &Memory[1])
            {
                Int32* remainsize = (Int32*)byt;
                *remainsize = Memory.Length - 5;
            }
        }
        public static LinearAllocator GetInstance()
        {
            if (Instance == null) { Instance = new LinearAllocator(); }
            return Instance;
        }

        /////
        public void* MemAlloc(int size)
        {
            if (size % 4 != 0) return MemAlloc(size + 1);
            int iter = 0;
            while (true)
            {
                while (Memory[iter] == 1) { iter = NewIter(iter); }
                if (CheckBlockLessThatSize(iter, size))
                {
                    iter = NewIter(iter);
                    if (iter == Memory.Length) return null;
                }
                else break;
            }
            fixed (byte* addr = &Memory[iter + 5],
                         byt = &Memory[iter + 1], 
                         newbyt = &Memory[iter + 6 + size])
            {
                Memory[iter] = 1;
                Int32* block = (Int32*)byt;
                Int32* newblock = (Int32*)newbyt;
                *newblock = *block - size - 5 ;
                *block = size;
                return addr;
            }
        }
        public void *MemReAlloc(void *Addr, int newsize)
        {
            if (Addr == null) return MemAlloc(newsize);

            void* Addres = MemAlloc(newsize);
            if (Addres == null) return null;

            int olditer = IterOfBlock(Addr);
            if (olditer == -1) return null;

            int size;
            fixed (byte *byt = &Memory[olditer + 1])
            {
                Int32* oldsize = (Int32*)byt;
                size = *oldsize < newsize ? *oldsize : newsize;
            }
            int newiter = IterOfBlock(Addres);
            for (int i = 0; i < size; i++)
            {
                Memory[newiter + 5 + i] = Memory[olditer + 5 + i];
            }
            MemFree(Addr);
            return Addres;
        }
        public void MemFree(void *Addr) 
        {
            int iter = IterOfBlock(Addr);
            if (iter == -1) return;
            Memory[iter] = 0;
            SplitBlocks();
        }
        public void Dump()
        {
            foreach (byte item in Memory)
            {
                Console.Write("{0} ", item);
            }
        }
        //////////
        private bool CheckBlockLessThatSize(int iter, int size)
        {
            fixed (byte* byt = &Memory[iter + 1])
            {
                Int32* block = (Int32*)byt;
                return *block < size;
            }
        }
        private int NewIter(int iter)
        {
            fixed (byte* byt = &Memory[iter + 1])
            {
                Int32* incriter = (Int32*)byt;
                return iter + *incriter + 5;
            }
        }
        private int IterOfBlock(void* Addr)
        {
            int iter = 0;
            while (true) 
            {
                if (iter == Memory.Length) return -1;
                fixed (byte *block = &Memory[iter + 5])
                {
                    if (Addr == block) return iter;
                    iter = NewIter(iter);
                }
            }
        }
        private void SplitBlocks()
        {
            int firstiter = 0;
            int seconditer = NewIter(firstiter);
            while (true)
            { 
                if (seconditer == Memory.Length) return;
                if (Memory[firstiter] == 0 && Memory[seconditer] == 0)
                {
                    fixed (byte* firstbyt = &Memory[firstiter + 1],
                                 secondbyt = &Memory[seconditer + 1])
                    {
                        Int32* firstblocklength = (Int32*)firstbyt;
                        Int32* secondblocklength = (Int32*)secondbyt;
                        *firstblocklength = *firstblocklength + *secondblocklength + 5;
                    }
                }
                else
                firstiter = seconditer;
                seconditer = NewIter(seconditer);
            }
        }
    }
}