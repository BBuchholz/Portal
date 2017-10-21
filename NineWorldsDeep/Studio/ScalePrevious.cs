namespace NineWorldsDeep.Studio
{
    public class ScalePrevious
    {

        #region fields

        protected string scaleName;
        protected PatternSignature patternSignature;

        #endregion
        

        public ScalePrevious(string name)
        {
            this.scaleName = name;
            this.patternSignature =
                ParseNameToPatternSignature(scaleName);
        }

        public PatternSignature PatternSignature { get { return patternSignature; } }

        public static PatternSignature ParseNameToPatternSignature(string name)
        {
            switch (name)
            {
                case "Major":
                    return new PatternSignature("0,2,4,5,7,9,11,12");
                case "Minor":
                    return new PatternSignature("0,2,3,5,7,8,10,12");
                default:
                    throw new InvalidScaleParseException();
            }
        }

        public override string ToString()
        {
            return scaleName;
        }

    }
}