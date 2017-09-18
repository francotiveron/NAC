using Nac.Common;
using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public static class NacFactory {
    public static NacObject Create(string parentPath) {
        NacObject nacObject = null;
        string name = Guid.NewGuid().ToString();

        NacPath p = new NacPath(parentPath);

        if (p.IsDatabase) {
            NacTag variable = new NacTag() { Name = name, Path = p.Child(name) };
            nacObject = variable;
        }
        else if (p.IsSection) {
            NacBlock block = new NacBlock() { Name = name, Path = p.Child(name) };
            nacObject = block;
        }
        else if (p.IsTask) {
            NacSection section = new NacSection() { Name = name, Path = p.Child(name) };
            nacObject = section;
        }
        else if (p.IsProject) {
            NacTask task = new NacTask() { Name = name, Path = p.Child(name) };
            nacObject = task;
        }
        else if (p.IsRoot) {
            NacProject project = new NacProject() { Name = name, Path = p.Child(name) };
            //project.Database.Path = project.Path + @"//";
            //project.Database.Name = project.Name + @"//";
            nacObject = project;
        }
        return nacObject;
    }
    public static NacBlock CreateBlock(string blockType, string sectionPath, Point position) {
        NacBlock block = null;

        switch (blockType) {
            case "Simple": block = new NacBlock(); break;
            case "Sequence": block = new NacBlockSeq(); break;
            case "If": block = new NacBlockIf(); break;
            case "Fuzzy": block = new NacBlockFuzzy(); break;
            case "Call": block = new NacBlockCall(); break;
            case "Timer": block = new NacBlockTimer(); break;
        }

        block.Name = Guid.NewGuid().ToString();
        block.Path = NacPath.Parse(sectionPath).Child(block.Name);
        block.Position = position;
        return block;
    }

}
