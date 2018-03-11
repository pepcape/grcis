using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace _041lsystems
{
  /// <summary>
  /// Interface for L-system rules.
  /// </summary>
  public class LSystemRule
  {
    public class RuleRightSide
    {
      public char left;
      public float weight;
      public string rule;

      override public string ToString ()
      {
        return left + " (" + weight.ToString( "F03" ) + ") ==> " + rule;
      }
    }

    public LSystemRule ( char aVariable, float aWeight, string aRuleRightSide )
    {
      mLeft = aVariable;
      mRightSides = new List<RuleRightSide>();
      RuleRightSide rightSide = new RuleRightSide();
      rightSide.weight = aWeight;
      rightSide.rule = aRuleRightSide;
      rightSide.left = mLeft;
      mWeightSum = aWeight;
      mRightSides.Add( rightSide );
    }

    public bool AddRightSide ( float aWeight, string aRuleRightSide )
    {
      RuleRightSide rightSide = new RuleRightSide();
      rightSide.weight = aWeight;
      rightSide.rule = aRuleRightSide;
      rightSide.left = mLeft;
      foreach ( RuleRightSide r in mRightSides )
        if ( r.rule == aRuleRightSide )
          return false;
      mWeightSum += aWeight;
      mRightSides.Add( rightSide );
      mRightSides.Sort( delegate( RuleRightSide r1, RuleRightSide r2 ) { return r1.rule.CompareTo( r2.rule ); } );
      return true;
    }

    public bool RemoveRightSide ( LSystemRule.RuleRightSide aRightSide )
    {
      if ( aRightSide == null )
        return false;
      float weight = aRightSide.weight;
      if ( mRightSides.Remove( aRightSide ) )
      {
        mWeightSum -= weight;
        return true;
      }
      return false;
    }

    public string ApplyRule ( double aRand )
    {
      double val = aRand * mWeightSum;
      double tmp = 0.0;
      foreach ( RuleRightSide i in mRightSides )
      {
        tmp += i.weight;
        if ( tmp >= val )
          return i.rule;
      }
      return "";
    }

    public void WriteRuleToXml ( XmlTextWriter aWriter )
    {
      NumberFormatInfo provider = new NumberFormatInfo();
      provider.NumberDecimalSeparator = ".";
      foreach ( RuleRightSide rside in mRightSides )
      {
        aWriter.WriteWhitespace( "  " );
        aWriter.WriteStartElement( "rule" );
        aWriter.WriteAttributeString( "leftSide", mLeft.ToString() );
        aWriter.WriteAttributeString( "weight", rside.weight.ToString( provider ) );
        aWriter.WriteAttributeString( "rightSide", rside.rule );
        aWriter.WriteEndElement();
        aWriter.WriteWhitespace( Environment.NewLine );
      }
    }

    public char mLeft;
    public float mWeightSum;
    public List<RuleRightSide> mRightSides;
  }

  public class LSystem
  {
    public LSystem ()
    {
      mRules = new Dictionary<char, LSystemRule>();
      mRandom = new Random();
    }

    public void Reset ()
    {
      start = '\0';
      mRules.Clear();
    }

    public string GetVariables ()
    {
      string variables = "";
      foreach ( KeyValuePair<char, LSystemRule> ruleRec in mRules )
        variables += ruleRec.Key;
      return variables;
    }

    public char start
    {
      get
      {
        return mStart;
      }
      set
      {
        mStart = value;
      }
    }

    public string Rewrite ( char aVariable )
    {
      if ( !mRules.ContainsKey( aVariable ) )
        return aVariable.ToString();
      return mRules[ aVariable ].ApplyRule( mRandom.NextDouble() );
    }

    public bool AddRule ( char aVariable, float aWeight, string aRuleRightSide )
    {
      LSystemRule rule;
      if ( !mRules.ContainsKey( aVariable ) )
      {
        rule = new LSystemRule( aVariable, aWeight, aRuleRightSide );
        mRules.Add( aVariable, rule );
        return true;
      }
      rule = mRules[ aVariable ];
      return rule.AddRightSide( aWeight, aRuleRightSide );
    }

    public bool RemoveRule ( LSystemRule.RuleRightSide aRightSide )
    {
      if ( aRightSide == null )
        return false;
      LSystemRule rule = mRules[ aRightSide.left ];
      if ( rule == null )
        return false;
      return rule.RemoveRightSide( aRightSide );
    }

    public bool ChangeRule ( LSystemRule.RuleRightSide aRightSide, char aVariable, float aWeight, string aRuleRightSide )
    {
      return RemoveRule( aRightSide ) && AddRule( aVariable, aWeight, aRuleRightSide );
    }

    public bool LoadFromFile ( string aFileName )
    {
      try
      {
        Reset();
        NumberFormatInfo provider = new NumberFormatInfo();
        provider.NumberDecimalSeparator = ".";
        XmlTextReader reader = new XmlTextReader( aFileName );

        while ( reader.Read() )
        {
          reader.MoveToElement();
          if ( reader.Name.ToString() == "axiom" )
          {
            reader.MoveToFirstAttribute();
            if ( reader.Name.ToString() == "name" )
            {
              string val = reader.Value;
              if ( val != null && val.Length > 0 )
                start = val[ 0 ];
            }
          }
          if ( reader.Name.ToString() == "rule" )
          {
            char lside = '\0';
            double weight = 0;
            string rside = null;
            while ( reader.MoveToNextAttribute() )
            {
              if ( reader.Name.ToString() == "leftSide" )
              {
                string val = reader.Value;
                if ( val != null && val.Length > 0 )
                  lside = val[ 0 ];
              }
              if ( reader.Name.ToString() == "rightSide" )
                rside = reader.Value;
              if ( reader.Name.ToString() == "weight" )
              {
                string val = reader.Value;
                weight = Convert.ToDouble( val, provider );
              }
            }
            if ( lside != '\0' && rside != null && weight > 0 )
              AddRule( lside, (float)weight, rside );
          }
        }
        reader.Close();
        return true;
      }
      catch ( Exception )
      {
        return false;
      }
    }

    public bool SaveToFile ( string aFileName )
    {
      try
      {
        XmlTextWriter writer = new XmlTextWriter( aFileName, null );
        //writer.WriteStartDocument();

        writer.WriteStartElement( "lsystem" );
        writer.WriteWhitespace( Environment.NewLine );

        writer.WriteWhitespace( "  " );
        writer.WriteStartElement( "axiom" );
        writer.WriteAttributeString( "name", mStart.ToString() );
        writer.WriteEndElement();
        writer.WriteWhitespace( Environment.NewLine );

        foreach ( KeyValuePair<char, LSystemRule> ruleRec in mRules )
          ruleRec.Value.WriteRuleToXml( writer );

        writer.WriteEndElement();
        writer.WriteWhitespace( Environment.NewLine );

        //writer.WriteEndDocument();
        writer.Close();
        return true;
      }
      catch ( Exception )
      {
        return false;
      }
    }

    public Dictionary<char, LSystemRule> rules
    {
      get
      {
        return mRules;
      }
    }

    protected char mStart;
    //protected System.Collections.Generic.List<char> mAlphabet;
    protected Dictionary<char, LSystemRule> mRules;
    protected Random mRandom;
  };

  public class LSystemGenerator
  {
    public void Generate ( LSystem aLSystem, int aIterationCount, out List<string> aIterations )
    {
      aIterations = new List<string>();
      string last = aLSystem.start.ToString();
      aIterations.Add( last );

      for ( int i = 1; i < aIterationCount; ++i )
      {
        string current = "";

        foreach ( char variable in last )
          current += aLSystem.Rewrite( variable );
        aIterations.Add( current );
        last = current;
      }
    }
  }
}
