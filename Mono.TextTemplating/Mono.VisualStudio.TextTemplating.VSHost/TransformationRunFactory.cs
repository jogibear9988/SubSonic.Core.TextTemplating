using System;
using Mono.TextTemplating;

namespace Mono.VisualStudio.TextTemplating.VSHost
{ 
	[Serializable]
	public abstract class TransformationRunFactory
		: IProcessTransformationRunFactory
	{
		public const string TransformationRunFactoryService = "TransformationRunFactoryService";
		public const string TransformationRunFactoryMethod = nameof(TransformationRunFactory);

		/// <summary>
		/// get the status of this instance.
		/// </summary>
		public bool IsAlive { get; set; }
		/// <summary>
		/// Create the transformation runner
		/// </summary>
		/// <param name="runnerType"></param>
		/// <param name="pt"></param>
		/// <param name="resolver"></param>
		/// <returns></returns>
		/// <remarks>
		/// abstracted, just because I am uncertain on how this would run on multiple platforms. Also visual studio classes may be required to pull of correctly.
		/// </remarks>
		public abstract IProcessTransformationRun CreateTransformationRun (Type runnerType, ParsedTemplate pt, ResolveEventHandler resolver);

		public abstract string RunTransformation (IProcessTransformationRun transformationRun);
	}
}
