using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqExamples;

namespace lab8
{
    public class Topic{
        public static List<string> AllTopics {get;} = new List<string>();
        public int Id { get; private init; }
        public string Name { get; private init; }
        public Topic(string name){
            Name = name;
            if (!AllTopics.Contains(name))
                AllTopics.Add(name);
            Id = AllTopics.IndexOf(name);
        }

        public override string ToString(){
            return $"{Name}({Id})";
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public Gender Gender {get;set; }
        public bool Active { get; set; }
        public int  DepartmentId { get; set; }

        public List<Topic> Topics { get; set; }
        public Student(int id, int index, string name, Gender gender,bool active, 
            int departmentId,List<Topic> topics)
        {
            this.Id = id;
            this.Index = index;
            this.Name = name;
            this.Gender = gender;
            this.Active = active;
            this.DepartmentId = departmentId;
            this.Topics = topics;
        }

        public override string ToString()
        {
            var result = $"{Id,2}) {Index,5}, {Name,11}, {Gender,6},{(Active ? "active" : "no active"),9},{DepartmentId,2}, topics: ";
            foreach (var str in Topics)
                result += str + ", ";
            return result;
        }
    }


    public class DBStudent
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public Gender Gender {get;set; }
        public bool Active { get; set; }
        public int  DepartmentId { get; set; }

        public DBStudent(int id, int index, string name, Gender gender,bool active, 
            int departmentId)
        {
            this.Id = id;
            this.Index = index;
            this.Name = name;
            this.Gender = gender;
            this.Active = active;
            this.DepartmentId = departmentId;
        }

        public override string ToString()
        {
            var result = $"{Id,2}) {Index,5}, {Name,11}, {Gender,6},{(Active ? "active" : "no active"),9},{DepartmentId,2}";
            return result;
        }
    }

    public class StudentToTopic{
        public int StudentId {get; init;}
        public int TopicId {get; init;}
    }

    class Program
    {
        static void Main(string[] args)
        {
            //DivideStudentsIntoParts(3);
            //StatsOfTopicsFirst();
            //StatsOfTopicsSecond();
            //ShowGeneratingStudentsEasy();
            //ShowGeneratingStudentsA();
            //ShowGeneratingStudentsB();
            ShowGeneratingStudentsC();
            //ShowReflection();
        }

        private static void ShowReflection()
        {
            var m1 = Assembly.GetExecutingAssembly().CreateInstance(Type.GetType("MixedNumber").FullName, false,
					BindingFlags.CreateInstance, null, new object[]{1, 2}, null, null);
            var m2 = Assembly.GetExecutingAssembly().CreateInstance(Type.GetType("MixedNumber").FullName, false,
					BindingFlags.CreateInstance, null, new object[]{1, 6}, null, null);
            var m3 = Type.GetType("MixedNumber").GetMethod("op_Addition").Invoke(m1, new object[]{m1, m2});
            var m4 = Assembly.GetExecutingAssembly().CreateInstance(Type.GetType("MixedNumber").FullName, false,
					BindingFlags.CreateInstance, null, new object[]{-1, 3}, null, null);
            var m5 = Type.GetType("MixedNumber").GetMethod("op_Addition").Invoke(m3, new object[]{m3, m4});
            Console.WriteLine(m5);
        }

        private static void ShowGeneratingStudentsC()
        {
            var studentsWithTopics = Generator.GenerateStudentsWithTopicsEasy();
            var topics = studentsWithTopics.SelectMany(s=>s.Topics).ToHashSet().Select(t => new Topic(t));
            var students = studentsWithTopics
                .Select(s => new DBStudent(s.Id, s.Index, s.Name, s.Gender, s.Active, s.DepartmentId));
            var studentsToTopics = studentsWithTopics.SelectMany(s=>s.Topics.Select(t=>new StudentToTopic(){StudentId = s.Id, TopicId = topics.First(a=>a.Name == t).Id}));
            var joined = studentsToTopics.Join(students, st=>st.StudentId, s=>s.Id, (st,s) => new{Student = s, StudentToTopic = st}).Join(topics, s=>s.StudentToTopic.TopicId, t=>t.Id, (s, t)=>new{Student=s.Student, Topic=t});
            foreach(var j in joined)
                Console.WriteLine($"{j.Student}, {j.Topic}");
        }

        private static void ShowGeneratingStudentsB()
        {
            var studentsWithTopics = Generator.GenerateStudentsWithTopicsEasy();
            var students = studentsWithTopics
                .Select(s => new Student(s.Id, s.Index, s.Name, s.Gender, s.Active, s.DepartmentId, s.Topics.Select(t => new Topic(t)).ToList()));
            foreach(var stud in students)
                Console.WriteLine(stud);
        }

        private static void ShowGeneratingStudentsA()
        {
            var studentsWithTopics = Generator.GenerateStudentsWithTopicsEasy();
            var topics = studentsWithTopics.SelectMany(s=>s.Topics).ToHashSet().Select(t => new Topic(t));
            var students = studentsWithTopics
                .Select(s => new Student(s.Id, s.Index, s.Name, s.Gender, s.Active, s.DepartmentId, s.Topics.Select(t => topics.First(a => a.Name == t)).ToList()));
            foreach(var stud in students)
                Console.WriteLine(stud);
        }

        private static void ShowGeneratingStudentsEasy()
        {
            var topics = new List<string>(){"C#", "algorithms", "Java", "PHP", "C++", "fuzzy logic", "Basic", 
                "JavaScript", "neural networks", "web programming"}.Select(t => new Topic(t));
            var studentsWithTopics = Generator.GenerateStudentsWithTopicsEasy();
            var students = studentsWithTopics
                .Select(s => new Student(s.Id, s.Index, s.Name, s.Gender, s.Active, s.DepartmentId, s.Topics
                .Select(t => topics.First(a => a.Name == t))
                .ToList()));
            foreach(var stud in students)
                Console.WriteLine(stud);
        }

        private static void StatsOfTopicsSecond()
        {
            var students = Generator.GenerateStudentsWithTopicsEasy();
            var topics = students
                .SelectMany(s => s.Topics.Select(a=> new{Topic = a, Gender = s.Gender}))
                .GroupBy(t=>t)
                .Select(g=>new{Topic = g.Key.Topic, Gender = g.Key.Gender, Occurences = g.Count()})
                .OrderBy(g=>g.Gender)
                .ThenByDescending(g=>g.Occurences);
            foreach (var group in topics)
            {
                Console.WriteLine($"{group.Gender} {group.Topic} {group.Occurences}");
            }
        }

        private static void StatsOfTopicsFirst()
        {
            var students = Generator.GenerateStudentsWithTopicsEasy();
            var topics = students
                .SelectMany(s=>s.Topics)
                .GroupBy(t=>t)
                .Select(g=>new{Topic = g.Key, Occurences = g.Count()})
                .OrderByDescending(g=>g.Occurences);
            foreach (var group in topics)
            {
                Console.WriteLine(group.Topic + " " + group.Occurences);
            }
        }

        static void DivideStudentsIntoParts(int partSize){
            var students = Generator.GenerateStudentsWithTopicsEasy();
            int counter = 0;
            var resStud = students
                .OrderBy(s => s.Name)
                .ThenBy(s=>s.Index)
                .GroupBy(s => counter++ / partSize)
                .Select(g => new {Id = g.Key, Students = g.ToList()})
                .ToList();
            
            foreach (var group in resStud)
            {
                Console.WriteLine(group.Id);
                group.Students.ToList().ForEach(s => Console.WriteLine("  " + s));
            }
        }
    }
}
