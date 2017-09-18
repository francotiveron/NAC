using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyTest {
    class Sample1 {
        // linguistic labels (fuzzy sets) that compose the distances
        FuzzySet fsNear = new FuzzySet("Near",
            new TrapezoidalFunction(15, 50, TrapezoidalFunction.EdgeType.Right));
        FuzzySet fsMedium = new FuzzySet("Medium",
            new TrapezoidalFunction(15, 50, 60, 100));
        FuzzySet fsFar = new FuzzySet("Far",
            new TrapezoidalFunction(60, 100, TrapezoidalFunction.EdgeType.Left));

        // front distance (input)
        LinguisticVariable lvFront = new LinguisticVariable("FrontalDistance", 0, 120);
        lvFront.AddLabel( fsNear );
lvFront.AddLabel( fsMedium );
lvFront.AddLabel( fsFar );

// linguistic labels (fuzzy sets) that compose the angle
FuzzySet fsZero = new FuzzySet("Zero",
    new TrapezoidalFunction(-10, 5, 5, 10));
        FuzzySet fsLP = new FuzzySet("LittlePositive",
            new TrapezoidalFunction(5, 10, 20, 25));
        FuzzySet fsP = new FuzzySet("Positive",
            new TrapezoidalFunction(20, 25, 35, 40));
        FuzzySet fsVP = new FuzzySet("VeryPositive",
            new TrapezoidalFunction(35, 40, TrapezoidalFunction.EdgeType.Left));

        // angle
        LinguisticVariable lvAngle = new LinguisticVariable("Angle", -10, 50);
        lvAngle.AddLabel( fsZero );
lvAngle.AddLabel( fsLP );
lvAngle.AddLabel( fsP );
lvAngle.AddLabel( fsVP );

// the database
Database fuzzyDB = new Database();
        fuzzyDB.AddVariable( lvFront );
fuzzyDB.AddVariable( lvAngle );

// creating the inference system
InferenceSystem IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

        // going straight
        IS.NewRule( "Rule 1", "IF FrontalDistance IS Far THEN Angle IS Zero" );
// turning left
IS.NewRule( "Rule 2", "IF FrontalDistance IS Near THEN Angle IS Positive" );

...
// inference section

// setting inputs
IS.SetInput( "FrontalDistance", 20 );

// getting outputs
try
{
    FuzzyOutput fuzzyOutput = IS.ExecuteInference("Angle");

    // showing the fuzzy output
    foreach ( FuzzyOutput.OutputConstraint oc in fuzzyOutput.OutputList )
    {
        Console.WriteLine( oc.Label + " - " + oc.FiringStrength.ToString( ) );
    }
}
catch ( Exception )
{
   ...
}
    }
}
