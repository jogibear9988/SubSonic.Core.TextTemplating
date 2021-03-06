// 
// TextTemplatingCallback.cs
//  
// Author:
//       Kenneth Carter <kccarter32@gmail.com>
// 
// Copyright (c) 2020 SubSonic-Core. (https://github.com/SubSonic-Core)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.TextTemplating;

namespace Mono.VisualStudio.TextTemplating.VSHost
{
	[Serializable]
	public abstract class ProcessEngineHost
		: ITextTemplatingEngineHost
		, ITextTemplatingSessionHost
	{
		public ProcessEngineHost()
		{
			StandardAssemblyReferences = new List<string> ();
			StandardImports = new List<string> ();
			Callback = new TextTemplatingCallback(this);
		}

		public ITextTemplatingCallback Callback { get; }

		public TemplateErrorCollection Errors => Callback.Errors;

		public virtual IList<string> StandardAssemblyReferences { get; }

		public virtual IList<string> StandardImports { get; }

		string templateFile;

		public string TemplateFile { get => templateFile;
			set {
				templateFile = value;

				if (Session != null) {
					Session[nameof (TemplateFile)] = value;
				}
			}
		}
#pragma warning disable CA2227 // Collection properties should be read only
		public ITextTemplatingSession Session { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

		protected virtual void Initialize ()
		{
			if (StandardAssemblyReferences is List<string> references) {
				references.AddRange (new[]
				{
					ResolveAssemblyReference ("System"),
					ResolveAssemblyReference ("System.Core"),
					typeof (TemplatingEngine).Assembly.Location
				}.Distinct());
			}

			if (StandardImports is List<string> imports) {
				imports.AddRange (new[]
				{
					"System",
					"Mono.VisualStudio.TextTemplating"
				}.Distinct ());
			}
		}

		public virtual ITextTemplatingSession CreateSession ()
		{
			return new TextTemplatingSession ();
		}

#if FEATURE_APPDOMAINS
		public abstract AppDomain ProvideTemplatingAppDomain (string context);
#endif

		public abstract object GetHostOption (string optionName);

		public abstract bool LoadIncludeText (string requestFileName, out string content, out string location);

		public void LogError (string message, bool isWarning = default)
		{
			Errors.Add (new TemplateError () {
				Message = message,
				IsWarning = isWarning
			});
		}

		public void LogError (string message, Location location, bool isWarning = default)
		{
			Errors.Add (new TemplateError (message, location) {
				IsWarning = isWarning
			});
		}

		public void LogErrors (TemplateErrorCollection errors)
		{
			Errors.AddRange (errors);
		}

		public abstract string ResolveAssemblyReference (string assemblyReference);

		public abstract Type ResolveDirectiveProcessor (string processorName);

		public abstract string ResolveParameterValue (string directiveId, string processorName, string parameterName);

		public abstract string ResolvePath (string path);

		public void SetFileExtension (string extension)
		{
			Callback.SetFileExtension (extension);
		}

		public void SetOutputEncoding (Encoding encoding, bool fromOutputDirective)
		{
			Callback.SetOutputEncoding (encoding, fromOutputDirective);
		}

		public void SetTemplateOutput(string templateOutput)
		{
			Callback.SetTemplateOutput (templateOutput);
		}
	}
}
