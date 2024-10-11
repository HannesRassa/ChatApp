using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISA3Demos.Models.Enums;

namespace BackEnd.Models.Classes;

public record Question {
    public int QuestionID { get; init; }
    public required string QuestionText { get; init; }
    public List<string> PossibleAnswers { get; init; } = new(); 
    public required string CorrectAnswer { get; init; }
    public int TimeLimit { get; set; } 
    public QuestionHardness Hardness {get; init;}
}
