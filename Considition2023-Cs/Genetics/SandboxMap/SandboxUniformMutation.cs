using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Considition2023_Cs.Genetics.SandboxMap
{
    internal class SandboxUniformMutation : MutationBase
    {
        #region Fields
        private int[] m_mutableGenesIndexes;

        private readonly bool m_allGenesMutable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.UniformMutation"/> class.
        /// </summary>
        /// <param name="mutableGenesIndexes">Mutable genes indexes.</param>
        public SandboxUniformMutation(params int[] mutableGenesIndexes)
        {
            m_mutableGenesIndexes = mutableGenesIndexes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.UniformMutation"/> class.
        /// </summary>
        /// <param name="allGenesMutable">If set to <c>true</c> all genes are mutable.</param>
        public SandboxUniformMutation(bool allGenesMutable)
        {
            m_allGenesMutable = allGenesMutable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.UniformMutation"/> class.
        /// </summary>
        /// <remarks>Creates an instance of UniformMutation where some random genes will be mutated.</remarks>
        public SandboxUniformMutation() : this(false)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            ExceptionHelper.ThrowIfNull("chromosome", chromosome);

            var genesLength = chromosome.Length;

            if (m_mutableGenesIndexes == null || m_mutableGenesIndexes.Length == 0)
            {
                if (m_allGenesMutable)
                {
                    m_mutableGenesIndexes = Enumerable.Range(0, genesLength).ToArray();
                }
                else
                {
                    m_mutableGenesIndexes = RandomizationProvider.Current.GetInts(1, 0, genesLength);
                }
            }

            do
            {

                for (int i = 0; i < m_mutableGenesIndexes.Length; i++)
                {
                    var geneIndex = m_mutableGenesIndexes[i];

                    if (geneIndex >= genesLength)
                    {
                        throw new MutationException(this, "The chromosome has no gene on index {0}. The chromosome genes length is {1}.".With(geneIndex, genesLength));
                    }

                    if (RandomizationProvider.Current.GetDouble() <= probability)
                    {
                        chromosome.ReplaceGene(geneIndex, chromosome.GenerateGene(geneIndex));
                    }
                }
            } while (chromosome
                        .GetGenes()
                        .Select(g =>
                                    (((int, int, int))g.Value).Item1
                               )
                        .Distinct()
                        .Count() != chromosome.Length);
        }
        #endregion
    }
}
