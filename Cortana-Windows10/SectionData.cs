using System;
using System.Collections.Generic;

public class SectionData {
    private static Dictionary<string, Section> _sections;
    public static Dictionary<string, Section> Sections {
        get {
            if(_sections == null) {
                _sections = new Dictionary<string, Section>();
            }
            return _sections;
        }
        set {
            _sections = value;
        }
    }
}

public class Section {
    public string Title { get; set; }

    //Other properties
}
