﻿using System.Collections.Generic;

namespace Somewhere2.ApplicationState
{
    public class Database
    {
        // Items excluding notes
        public List<TagItem> SystemEntries { get; set; }
        // Notes don't have unique path
        public List<TagItem> Notes { get; set; }
    }
}