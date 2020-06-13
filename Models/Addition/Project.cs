﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using Employees.Classes;
using Employees.Models;
using LinqToDB;

namespace DataModels
{
    public partial class Project : ViewModelBase, ICloneable, ISearchable
    {
        public List<ProjectRequiredSkill> SkillsToAdd { get; set; } = new List<ProjectRequiredSkill>();
        public List<ProjectRequiredSkill> SkillsToDelete { get; set; } = new List<ProjectRequiredSkill>();
        public ObservableCollection<ProjectRequiredSkill> Skills { get; set; }
        
        public List<ProjectParticipant> ParticipantsToAdd { get; set; } = new List<ProjectParticipant>();
        public List<ProjectParticipant> ParticipantsToDelete { get; set; } = new List<ProjectParticipant>();
        public ObservableCollection<ProjectParticipant> Participants { get; set; }

        public void LoadSkills()
        {
            Requiredskillsidfks.ForEach(rs => rs.Skill = DBModel.SkillsTable.Find(rs.SkillId));
        }

        public void LoadParticipants()
        {
            Participantidfks.ForEach(p => p.Employee = DBModel.EmployeesTable.Find(p.EmployeeId));
        }

        public void AddSkill(Project project, Skill skill, short level)
        {
            var requiredSkill = new ProjectRequiredSkill
            {
                Project = project, 
                ProjectId = project.Id,
                Skill = skill,
                SkillId = skill.Id,
                Level = level
            };
            var index = SkillsToDelete.IndexOf(s => s == requiredSkill);
            if (index == -1)
                SkillsToAdd.Add(requiredSkill);
            else
                SkillsToDelete.RemoveAt(index);
        }

        public void AddParticipant(Project project, Employee employee)
        {
            var participant = new ProjectParticipant
            {
                Employee = employee,
                EmployeeId = employee.Id,
                Project = project,
                ProjectId = project.Id
            };
            var index = ParticipantsToDelete.IndexOf(s => s == participant);
            if (index == -1)
                ParticipantsToAdd.Add(participant);
            else
                ParticipantsToDelete.RemoveAt(index);
        }

        public void UpdateSkills()
        {
            var skills = new List<ProjectRequiredSkill>();
            if (!Requiredskillsidfks.IsEmpty() && !SkillsToAdd.IsEmpty())
                skills = Requiredskillsidfks.Union(SkillsToAdd).ToList();
            else if (!Requiredskillsidfks.IsEmpty())
                skills = Requiredskillsidfks.ToList();
            else if (!SkillsToAdd.IsEmpty())
                skills = SkillsToAdd;
            
            Skills = new ObservableCollection<ProjectRequiredSkill>(skills.Where(rs => SkillsToDelete.All(s => s != rs)));
            RaisePropertyChanged(nameof(Skills));
        }

        public void UpdateParticipants()
        {
            var participants = new List<ProjectParticipant>();
            if (!Participantidfks.IsEmpty() && !ParticipantsToAdd.IsEmpty())
                participants = Participantidfks.Union(ParticipantsToAdd).ToList();
            else if (!Participantidfks.IsEmpty())
                participants = Participantidfks.ToList();
            else if (!ParticipantsToAdd.IsEmpty())
                participants = ParticipantsToAdd;
            
            Participants = new ObservableCollection<ProjectParticipant>(participants.Where(p => ParticipantsToDelete.All(pp => pp != p)));
            RaisePropertyChanged(nameof(Participants));
        }

        public void SaveSkills(long id=0)
        {
            SkillsToAdd.ForEach(s =>
            {
                if (SkillsToDelete.Any(sd => sd == s)) return;
                if (id != 0)
                    s.ProjectId = id;
                DBModel.Context.Insert(s);
            });
            SkillsToDelete.ForEach(s =>
            {
                if (s.ProjectId != 0)
                    DBModel.EmployeesDB.Delete(s);
            });
        }

        public void SaveParticipants(long id=0)
        {
            ParticipantsToAdd.ForEach(s =>
            {
                if (ParticipantsToDelete.Any(sd => sd == s)) return;
                if (id != 0)
                    s.ProjectId = id;
                DBModel.Context.Insert(s);
            });
            ParticipantsToDelete.ForEach(s =>
            {
                if (s.ProjectId != 0)
                    DBModel.EmployeesDB.Delete(s);
            });
        }
        
        public object Clone() => MemberwiseClone();

        public bool Search(string search) => 
            search.IsEmpty() || Name.Search(search) || StartDate.Search(search) || FinishDate.Search(search);
    }
}