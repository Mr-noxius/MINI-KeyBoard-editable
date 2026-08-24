using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Reflection.Emit;

namespace System.Linq.Expressions.Compiler;

internal sealed class LabelInfo
{
	private readonly LabelTarget _node;

	private Label _label;

	private bool _labelDefined;

	private LocalBuilder _value;

	private readonly System.Linq.Expressions.Set<System.Linq.Expressions.Compiler.LabelScopeInfo> _definitions = new System.Linq.Expressions.Set<System.Linq.Expressions.Compiler.LabelScopeInfo>();

	private readonly List<System.Linq.Expressions.Compiler.LabelScopeInfo> _references = new List<System.Linq.Expressions.Compiler.LabelScopeInfo>();

	private readonly bool _canReturn;

	private bool _acrossBlockJump;

	private OpCode _opCode = OpCodes.Leave;

	private readonly ILGenerator _ilg;

	internal Label Label
	{
		get
		{
			EnsureLabelAndValue();
			return _label;
		}
	}

	internal bool CanReturn => _canReturn;

	internal bool CanBranch => _opCode != OpCodes.Leave;

	internal LabelInfo(ILGenerator il, LabelTarget node, bool canReturn)
	{
		_ilg = il;
		_node = node;
		_canReturn = canReturn;
	}

	internal void Reference(System.Linq.Expressions.Compiler.LabelScopeInfo block)
	{
		_references.Add(block);
		if (_definitions.Count > 0)
		{
			ValidateJump(block);
		}
	}

	internal void Define(System.Linq.Expressions.Compiler.LabelScopeInfo block)
	{
		for (System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo = block; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
		{
			if (labelScopeInfo.ContainsTarget(_node))
			{
				throw System.Linq.Expressions.Error.LabelTargetAlreadyDefined(_node.Name);
			}
		}
		_definitions.Add(block);
		block.AddLabelInfo(_node, this);
		if (_definitions.Count == 1)
		{
			foreach (System.Linq.Expressions.Compiler.LabelScopeInfo reference in _references)
			{
				ValidateJump(reference);
			}
			return;
		}
		if (_acrossBlockJump)
		{
			throw System.Linq.Expressions.Error.AmbiguousJump(_node.Name);
		}
		_labelDefined = false;
	}

	private void ValidateJump(System.Linq.Expressions.Compiler.LabelScopeInfo reference)
	{
		_opCode = (_canReturn ? OpCodes.Ret : OpCodes.Br);
		for (System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo = reference; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
		{
			if (_definitions.Contains(labelScopeInfo))
			{
				return;
			}
			if (labelScopeInfo.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Finally || labelScopeInfo.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Filter)
			{
				break;
			}
			if (labelScopeInfo.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Try || labelScopeInfo.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Catch)
			{
				_opCode = OpCodes.Leave;
			}
		}
		_acrossBlockJump = true;
		if (_node != null && _node.Type != typeof(void))
		{
			throw System.Linq.Expressions.Error.NonLocalJumpWithValue(_node.Name);
		}
		if (_definitions.Count > 1)
		{
			throw System.Linq.Expressions.Error.AmbiguousJump(_node.Name);
		}
		System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo2 = _definitions.First();
		System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo3 = System.Dynamic.Utils.Helpers.CommonNode(labelScopeInfo2, reference, (System.Linq.Expressions.Compiler.LabelScopeInfo b) => b.Parent);
		_opCode = (_canReturn ? OpCodes.Ret : OpCodes.Br);
		for (System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo4 = reference; labelScopeInfo4 != labelScopeInfo3; labelScopeInfo4 = labelScopeInfo4.Parent)
		{
			if (labelScopeInfo4.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Finally)
			{
				throw System.Linq.Expressions.Error.ControlCannotLeaveFinally();
			}
			if (labelScopeInfo4.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Filter)
			{
				throw System.Linq.Expressions.Error.ControlCannotLeaveFilterTest();
			}
			if (labelScopeInfo4.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Try || labelScopeInfo4.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Catch)
			{
				_opCode = OpCodes.Leave;
			}
		}
		for (System.Linq.Expressions.Compiler.LabelScopeInfo labelScopeInfo5 = labelScopeInfo2; labelScopeInfo5 != labelScopeInfo3; labelScopeInfo5 = labelScopeInfo5.Parent)
		{
			if (!labelScopeInfo5.CanJumpInto)
			{
				if (labelScopeInfo5.Kind == System.Linq.Expressions.Compiler.LabelScopeKind.Expression)
				{
					throw System.Linq.Expressions.Error.ControlCannotEnterExpression();
				}
				throw System.Linq.Expressions.Error.ControlCannotEnterTry();
			}
		}
	}

	internal void ValidateFinish()
	{
		if (_references.Count > 0 && _definitions.Count == 0)
		{
			throw System.Linq.Expressions.Error.LabelTargetUndefined(_node.Name);
		}
	}

	internal void EmitJump()
	{
		if (_opCode == OpCodes.Ret)
		{
			_ilg.Emit(OpCodes.Ret);
			return;
		}
		StoreValue();
		_ilg.Emit(_opCode, Label);
	}

	private void StoreValue()
	{
		EnsureLabelAndValue();
		if (_value != null)
		{
			_ilg.Emit(OpCodes.Stloc, _value);
		}
	}

	internal void Mark()
	{
		if (_canReturn)
		{
			if (!_labelDefined)
			{
				return;
			}
			_ilg.Emit(OpCodes.Ret);
		}
		else
		{
			StoreValue();
		}
		MarkWithEmptyStack();
	}

	internal void MarkWithEmptyStack()
	{
		_ilg.MarkLabel(Label);
		if (_value != null)
		{
			_ilg.Emit(OpCodes.Ldloc, _value);
		}
	}

	private void EnsureLabelAndValue()
	{
		if (!_labelDefined)
		{
			_labelDefined = true;
			_label = _ilg.DefineLabel();
			if (_node != null && _node.Type != typeof(void))
			{
				_value = _ilg.DeclareLocal(_node.Type);
			}
		}
	}
}
