﻿

using QLite.Dto.Kapp;

namespace QLite.Dto
{
    public class SegmentDto: KappBaseDto
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public string Prefix { get; set; }

    }
}