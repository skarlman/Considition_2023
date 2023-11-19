using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics.SandboxMap
{
    public class SandboxUniformCrossover : CrossoverBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.UniformCrossover"/> class.
        /// </summary>
        /// <param name="mixProbability">The mix probability. he default mix probability is 0.5.</param>
        public SandboxUniformCrossover(float mixProbability)
            : base(2, 2)
        {
            MixProbability = mixProbability;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.UniformCrossover"/> class.
        /// <remarks>
        /// The default mix probability is 0.5.
        /// </remarks>
        /// </summary>
        public SandboxUniformCrossover() : this(0.5f)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the mix probability.
        /// </summary>
        /// <value>The mix probability.</value>
        public float MixProbability { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var firstChild = firstParent.CreateNew();
            var secondChild = secondParent.CreateNew();

            do
            {
                for (int i = 0; i < firstParent.Length; i++)
                {
                    if (RandomizationProvider.Current.GetDouble() < MixProbability)
                    {
                        firstChild.ReplaceGene(i, firstParent.GetGene(i));
                        secondChild.ReplaceGene(i, secondParent.GetGene(i));
                    }
                    else
                    {
                        firstChild.ReplaceGene(i, secondParent.GetGene(i));
                        secondChild.ReplaceGene(i, firstParent.GetGene(i));
                    }
                }
            } while (firstChild
                        .GetGenes()
                        .Select(g =>
                                    (((int, int, int))g.Value).Item1
                               )
                        .Distinct()
                        .Count() != firstChild.Length
                    &&
                        secondChild
                        .GetGenes()
                        .Select(g =>
                                    (((int, int, int))g.Value).Item1
                               )
                        .Distinct()
                        .Count() != secondChild.Length);

            return new List<IChromosome> { firstChild, secondChild };
        }
        #endregion
    }
}
