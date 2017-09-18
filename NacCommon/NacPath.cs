using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common {
    public struct NacPath {
        public const char Separator = '/';
        public const string DBMark = @"//";

        public static NacPath Parse(string path) { return new NacPath(path); }

        public static string GetParentOf(string path) {
            NacPath p = new NacPath(path);
            return p.Parent;
        }
        public static string GetParentOf(NacObject o) {
            return GetParentOf(o.Path);
        }
        public static string ReplaceLeaf(string path, string leaf) {
            return GetParentOf(path) + Separator + leaf;
        }

        public static string Replace(string path, string newRoot = null, string newProject = null) {
            var nacPath = new NacPath(path);
            if (newRoot != null) nacPath.EngineUrl = newRoot;
            if (newProject != null) nacPath.ProjectPart = newProject;
            return nacPath;
        }

        public int Level {
            get {
                switch (Class) {
                    case NacObjectClass.NacRoot: return 0;
                    case NacObjectClass.NacProject: return 1;
                    case NacObjectClass.NacDatabase: return 2;
                    case NacObjectClass.NacTag: return 3;
                    case NacObjectClass.NacTask: return 2;
                    case NacObjectClass.NacSection: return 3;
                    case NacObjectClass.NacBlock: return 4;
                    default: return -1;
                }
            }
        }

        public static string Build(params string[] parts) {
            return string.Join(Separator.ToString(), parts);
        }

        public string Parent {
            get {
                switch(Class) {
                    case NacObjectClass.NacTag: return DatabasePath;
                    case NacObjectClass.NacTask: return ProjectPath;
                    case NacObjectClass.NacSection: return TaskPath;
                    case NacObjectClass.NacBlock: return SectionPath;
                    case NacObjectClass.NacProject: return EngineUrl;
                }
                return null;
            }
        }
        public NacObjectClass Class {
            get {
                if (IsRoot) return NacObjectClass.NacRoot;
                if (IsProject) return NacObjectClass.NacProject;
                if (IsDatabase) return NacObjectClass.NacDatabase;
                if (IsTag) return NacObjectClass.NacTag;
                if (IsTask) return NacObjectClass.NacTask;
                if (IsSection) return NacObjectClass.NacSection;
                if (IsBlock) return NacObjectClass.NacBlock;
                return NacObjectClass.Unknown;
            }
        }

        public string EngineUrl { get; set; }
        public string ProjectPart { get; set; }
        public string TagPart { get; set; }
        public string TaskPart { get; set; }
        public string SectionPart { get; set; }
        public string BlockPart { get; set; }

        public string ProjectPath { get { return ProjectPart == null ? null : Build(EngineUrl, ProjectPart); } }
        public string DatabasePath { get { return ProjectPart == null ? null : ProjectPath + DBMark; } }
        public string TagPath { get { return TagPart == null ? null : DatabasePath + TagPart; } }
        public string TaskPath { get { return TaskPart == null ? null : Build(ProjectPath, TaskPart); } }
        public string SectionPath { get { return SectionPart == null ? null : Build(TaskPath, SectionPart); } }
        public string BlockPath { get { return IsBlock ? Build(SectionPath, BlockPart) : null; } }

        public bool IsProject { get { return ProjectPart != null && Part(2) == null; } }
        public bool IsTask { get { return TaskPart != null && SectionPart == null; } }
        public bool IsSection { get { return SectionPart != null && BlockPart == null; } }
        public bool IsBlock { get { return BlockPart != null; } }
        public bool IsRoot { get { return ProjectPart == null; } }
        public bool IsDatabase { get { return ProjectPart != null && Part(2) == string.Empty && TagPart == null; } }
        public bool IsTag { get { return TagPart != null; } }

        string[] _parts;
        private string Part(int i) { try { return i >= _parts.Length ? null : _parts[i]; } catch { return null; } }
        public string Path {
            get {
                string ret = EngineUrl;
                if (ProjectPart != null) ret += Separator + ProjectPart;
                if (Part(2) == string.Empty) ret += DBMark;
                if (TagPart != null) ret += TagPart;
                else {
                    if (TaskPart != null) ret += Separator + TaskPart;
                    if (SectionPart != null) ret += Separator + SectionPart;
                    if (BlockPart != null) ret += Separator + BlockPart;
                }
                return ret;
            }
            set {
                Uri u;
                Uri.TryCreate(value, UriKind.Absolute, out u);
                if (u == null) {
                    EngineUrl = null;
                    _parts = value.Split(Separator);
                }
                else {
                    EngineUrl = u.Scheme + "://" + u.Authority;
                    _parts = u.AbsolutePath.Split(Separator);
                }
                ProjectPart = string.IsNullOrEmpty(Part(1)) ? null: Part(1);
                if (Part(2) == string.Empty) {
                    TagPart = string.IsNullOrEmpty(Part(3)) ? null : Part(3);
                    TaskPart = SectionPart = BlockPart = null;
                }
                else {
                    TaskPart = Part(2);
                    SectionPart = Part(3);
                    BlockPart = Part(4);
                    TagPart = null;
                }
            }
        }

        public string Child(string name) {
            return IsDatabase ? Path + name : Build(Path, name);
        }
        public NacPath(string path) {
            EngineUrl = ProjectPart = TagPart = TaskPart = SectionPart = BlockPart = null;
            _parts = null;
            Path = path;
        }
        public static implicit operator string(NacPath path) { return path.Path; }
        public static implicit operator NacPath(string path) { return new NacPath(path); }
    }
}
