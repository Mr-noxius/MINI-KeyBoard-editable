using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler;

internal sealed class LabelScopeInfo
{
	private Dictionary<LabelTarget, System.Linq.Expressions.Compiler.LabelInfo> _labels;

	internal readonly System.Linq.Expressions.Compiler.LabelScopeKind Kind;

	internal readonly System.Linq.Expressions.Compiler.LabelScopeInfo Parent;

	internal bool CanJumpInto
	{
		get
		{
			switch (Kind)
			{
			case System.Linq.Expressions.Compiler.LabelScopeKind.Statement:
			case System.Linq.Expressions.Compiler.LabelScopeKind.Block:
			case System.Linq.Expressions.Compiler.LabelScopeKind.Switch:
			case System.Linq.Expressions.Compiler.LabelScopeKind.Lambda:
				return true;
			default:
				return false;
			}
		}
	}

	internal LabelScopeInfo(System.Linq.Expressions.Compiler.LabelScopeInfo parent, System.Linq.Expressions.Compiler.LabelScopeKind kind)
	{
		Parent = parent;
		Kind = kind;
	}

	internal bool ContainsTarget(LabelTarget target)
	{
		if (_labels == null)
		{
			return false;
		}
		return _labels.ContainsKey(target);
	}

	internal bool TryGetLabelInfo(LabelTarget target, out System.Linq.Expressions.Compiler.LabelInfo info)
	{
		if (_labels == null)
		{
			info = null;
			return false;
		}
		return _labels.TryGetValue(target, out info);
	}

	internal void AddLabelInfo(LabelTarget target, System.Linq.Expressions.Compiler.LabelInfo info)
	{
		if (_labels == null)
		{
			_labels = new Dictionary<LabelTarget, System.Linq.Expressions.Compiler.LabelInfo>();
		}
		_labels.Add(target, info);
	}
}
