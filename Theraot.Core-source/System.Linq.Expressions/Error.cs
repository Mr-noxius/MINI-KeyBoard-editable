using System.Dynamic.Utils;

namespace System.Linq.Expressions;

internal static class Error
{
	internal static Exception ReducibleMustOverrideReduce()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ReducibleMustOverrideReduce);
	}

	internal static Exception MustReduceToDifferent()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MustReduceToDifferent);
	}

	internal static Exception ReducedNotCompatible()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ReducedNotCompatible);
	}

	internal static Exception SetterHasNoParams()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.SetterHasNoParams);
	}

	internal static Exception PropertyCannotHaveRefType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyCannotHaveRefType);
	}

	internal static Exception IndexesOfSetGetMustMatch()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IndexesOfSetGetMustMatch);
	}

	internal static Exception AccessorsCannotHaveVarArgs()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.AccessorsCannotHaveVarArgs);
	}

	internal static Exception AccessorsCannotHaveByRefArgs()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.AccessorsCannotHaveByRefArgs);
	}

	internal static Exception BoundsCannotBeLessThanOne()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.BoundsCannotBeLessThanOne);
	}

	internal static Exception TypeMustNotBeByRef()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TypeMustNotBeByRef);
	}

	internal static Exception TypeDoesNotHaveConstructorForTheSignature()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TypeDoesNotHaveConstructorForTheSignature);
	}

	internal static Exception CountCannotBeNegative()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.CountCannotBeNegative);
	}

	internal static Exception ArrayTypeMustBeArray()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArrayTypeMustBeArray);
	}

	internal static Exception SetterMustBeVoid()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.SetterMustBeVoid);
	}

	internal static Exception PropertyTyepMustMatchSetter()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyTyepMustMatchSetter);
	}

	internal static Exception BothAccessorsMustBeStatic()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.BothAccessorsMustBeStatic);
	}

	internal static Exception OnlyStaticMethodsHaveNullInstance()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.OnlyStaticMethodsHaveNullInstance);
	}

	internal static Exception PropertyTypeCannotBeVoid()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyTypeCannotBeVoid);
	}

	internal static Exception InvalidUnboxType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InvalidUnboxType);
	}

	internal static Exception ArgumentMustNotHaveValueType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustNotHaveValueType);
	}

	internal static Exception MustBeReducible()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MustBeReducible);
	}

	internal static Exception DefaultBodyMustBeSupplied()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.DefaultBodyMustBeSupplied);
	}

	internal static Exception MethodBuilderDoesNotHaveTypeBuilder()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MethodBuilderDoesNotHaveTypeBuilder);
	}

	internal static Exception LabelMustBeVoidOrHaveExpression()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.LabelMustBeVoidOrHaveExpression);
	}

	internal static Exception LabelTypeMustBeVoid()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.LabelTypeMustBeVoid);
	}

	internal static Exception QuotedExpressionMustBeLambda()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.QuotedExpressionMustBeLambda);
	}

	internal static Exception VariableMustNotBeByRef(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.VariableMustNotBeByRef(p0, p1));
	}

	internal static Exception DuplicateVariable(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.DuplicateVariable(p0));
	}

	internal static Exception StartEndMustBeOrdered()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.StartEndMustBeOrdered);
	}

	internal static Exception FaultCannotHaveCatchOrFinally()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.FaultCannotHaveCatchOrFinally);
	}

	internal static Exception TryMustHaveCatchFinallyOrFault()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TryMustHaveCatchFinallyOrFault);
	}

	internal static Exception BodyOfCatchMustHaveSameTypeAsBodyOfTry()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.BodyOfCatchMustHaveSameTypeAsBodyOfTry);
	}

	internal static Exception ExtensionNodeMustOverrideProperty(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ExtensionNodeMustOverrideProperty(p0));
	}

	internal static Exception UserDefinedOperatorMustBeStatic(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UserDefinedOperatorMustBeStatic(p0));
	}

	internal static Exception UserDefinedOperatorMustNotBeVoid(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UserDefinedOperatorMustNotBeVoid(p0));
	}

	internal static Exception CoercionOperatorNotDefined(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CoercionOperatorNotDefined(p0, p1));
	}

	internal static Exception UnaryOperatorNotDefined(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.UnaryOperatorNotDefined(p0, p1));
	}

	internal static Exception BinaryOperatorNotDefined(object p0, object p1, object p2)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.BinaryOperatorNotDefined(p0, p1, p2));
	}

	internal static Exception ReferenceEqualityNotDefined(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ReferenceEqualityNotDefined(p0, p1));
	}

	internal static Exception OperandTypesDoNotMatchParameters(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.OperandTypesDoNotMatchParameters(p0, p1));
	}

	internal static Exception OverloadOperatorTypeDoesNotMatchConversionType(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.OverloadOperatorTypeDoesNotMatchConversionType(p0, p1));
	}

	internal static Exception ConversionIsNotSupportedForArithmeticTypes()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ConversionIsNotSupportedForArithmeticTypes);
	}

	internal static Exception ArgumentMustBeArray()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeArray);
	}

	internal static Exception ArgumentMustBeBoolean()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeBoolean);
	}

	internal static Exception EqualityMustReturnBoolean(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.EqualityMustReturnBoolean(p0));
	}

	internal static Exception ArgumentMustBeFieldInfoOrPropertInfo()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeFieldInfoOrPropertInfo);
	}

	internal static Exception ArgumentMustBeFieldInfoOrPropertInfoOrMethod()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeFieldInfoOrPropertInfoOrMethod);
	}

	internal static Exception ArgumentMustBeInstanceMember()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeInstanceMember);
	}

	internal static Exception ArgumentMustBeInteger()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeInteger);
	}

	internal static Exception ArgumentMustBeArrayIndexType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeArrayIndexType);
	}

	internal static Exception ArgumentMustBeSingleDimensionalArrayType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMustBeSingleDimensionalArrayType);
	}

	internal static Exception ArgumentTypesMustMatch()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentTypesMustMatch);
	}

	internal static Exception CannotAutoInitializeValueTypeElementThroughProperty(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CannotAutoInitializeValueTypeElementThroughProperty(p0));
	}

	internal static Exception CannotAutoInitializeValueTypeMemberThroughProperty(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CannotAutoInitializeValueTypeMemberThroughProperty(p0));
	}

	internal static Exception IncorrectTypeForTypeAs(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectTypeForTypeAs(p0));
	}

	internal static Exception CoalesceUsedOnNonNullType()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CoalesceUsedOnNonNullType);
	}

	internal static Exception ExpressionTypeCannotInitializeArrayType(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ExpressionTypeCannotInitializeArrayType(p0, p1));
	}

	internal static Exception ExpressionTypeDoesNotMatchConstructorParameter(object p0, object p1)
	{
		return Error.ExpressionTypeDoesNotMatchConstructorParameter(p0, p1);
	}

	internal static Exception ArgumentTypeDoesNotMatchMember(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentTypeDoesNotMatchMember(p0, p1));
	}

	internal static Exception ArgumentMemberNotDeclOnType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentMemberNotDeclOnType(p0, p1));
	}

	internal static Exception ExpressionTypeDoesNotMatchMethodParameter(object p0, object p1, object p2)
	{
		return Error.ExpressionTypeDoesNotMatchMethodParameter(p0, p1, p2);
	}

	internal static Exception ExpressionTypeDoesNotMatchParameter(object p0, object p1)
	{
		return Error.ExpressionTypeDoesNotMatchParameter(p0, p1);
	}

	internal static Exception ExpressionTypeDoesNotMatchReturn(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ExpressionTypeDoesNotMatchReturn(p0, p1));
	}

	internal static Exception ExpressionTypeDoesNotMatchAssignment(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ExpressionTypeDoesNotMatchAssignment(p0, p1));
	}

	internal static Exception ExpressionTypeDoesNotMatchLabel(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ExpressionTypeDoesNotMatchLabel(p0, p1));
	}

	internal static Exception ExpressionTypeNotInvocable(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ExpressionTypeNotInvocable(p0));
	}

	internal static Exception FieldNotDefinedForType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.FieldNotDefinedForType(p0, p1));
	}

	internal static Exception InstanceFieldNotDefinedForType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InstanceFieldNotDefinedForType(p0, p1));
	}

	internal static Exception FieldInfoNotDefinedForType(object p0, object p1, object p2)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.FieldInfoNotDefinedForType(p0, p1, p2));
	}

	internal static Exception IncorrectNumberOfIndexes()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfIndexes);
	}

	internal static Exception IncorrectNumberOfLambdaArguments()
	{
		return Error.IncorrectNumberOfLambdaArguments();
	}

	internal static Exception IncorrectNumberOfLambdaDeclarationParameters()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfLambdaDeclarationParameters);
	}

	internal static Exception IncorrectNumberOfMethodCallArguments(object p0)
	{
		return Error.IncorrectNumberOfMethodCallArguments(p0);
	}

	internal static Exception IncorrectNumberOfConstructorArguments()
	{
		return Error.IncorrectNumberOfConstructorArguments();
	}

	internal static Exception IncorrectNumberOfMembersForGivenConstructor()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfMembersForGivenConstructor);
	}

	internal static Exception IncorrectNumberOfArgumentsForMembers()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfArgumentsForMembers);
	}

	internal static Exception LambdaTypeMustBeDerivedFromSystemDelegate()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.LambdaTypeMustBeDerivedFromSystemDelegate);
	}

	internal static Exception MemberNotFieldOrProperty(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MemberNotFieldOrProperty(p0));
	}

	internal static Exception MethodContainsGenericParameters(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MethodContainsGenericParameters(p0));
	}

	internal static Exception MethodIsGeneric(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MethodIsGeneric(p0));
	}

	internal static Exception MethodNotPropertyAccessor(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.MethodNotPropertyAccessor(p0, p1));
	}

	internal static Exception PropertyDoesNotHaveGetter(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyDoesNotHaveGetter(p0));
	}

	internal static Exception PropertyDoesNotHaveSetter(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyDoesNotHaveSetter(p0));
	}

	internal static Exception PropertyDoesNotHaveAccessor(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyDoesNotHaveAccessor(p0));
	}

	internal static Exception NotAMemberOfType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.NotAMemberOfType(p0, p1));
	}

	internal static Exception ExpressionNotSupportedForType(object p0, object p1)
	{
		return new PlatformNotSupportedException(System.Linq.Expressions.Strings.ExpressionNotSupportedForType(p0, p1));
	}

	internal static Exception ExpressionNotSupportedForNullableType(object p0, object p1)
	{
		return new PlatformNotSupportedException(System.Linq.Expressions.Strings.ExpressionNotSupportedForNullableType(p0, p1));
	}

	internal static Exception ParameterExpressionNotValidAsDelegate(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ParameterExpressionNotValidAsDelegate(p0, p1));
	}

	internal static Exception PropertyNotDefinedForType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.PropertyNotDefinedForType(p0, p1));
	}

	internal static Exception InstancePropertyNotDefinedForType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InstancePropertyNotDefinedForType(p0, p1));
	}

	internal static Exception InstancePropertyWithoutParameterNotDefinedForType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InstancePropertyWithoutParameterNotDefinedForType(p0, p1));
	}

	internal static Exception InstancePropertyWithSpecifiedParametersNotDefinedForType(object p0, object p1, object p2)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InstancePropertyWithSpecifiedParametersNotDefinedForType(p0, p1, p2));
	}

	internal static Exception InstanceAndMethodTypeMismatch(object p0, object p1, object p2)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InstanceAndMethodTypeMismatch(p0, p1, p2));
	}

	internal static Exception TypeMissingDefaultConstructor(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TypeMissingDefaultConstructor(p0));
	}

	internal static Exception ListInitializerWithZeroMembers()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ListInitializerWithZeroMembers);
	}

	internal static Exception ElementInitializerMethodNotAdd()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ElementInitializerMethodNotAdd);
	}

	internal static Exception ElementInitializerMethodNoRefOutParam(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ElementInitializerMethodNoRefOutParam(p0, p1));
	}

	internal static Exception ElementInitializerMethodWithZeroArgs()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ElementInitializerMethodWithZeroArgs);
	}

	internal static Exception ElementInitializerMethodStatic()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ElementInitializerMethodStatic);
	}

	internal static Exception TypeNotIEnumerable(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TypeNotIEnumerable(p0));
	}

	internal static Exception UnexpectedCoalesceOperator()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.UnexpectedCoalesceOperator);
	}

	internal static Exception InvalidCast(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.InvalidCast(p0, p1));
	}

	internal static Exception UnhandledBinary(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledBinary(p0));
	}

	internal static Exception UnhandledBinding()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledBinding);
	}

	internal static Exception UnhandledBindingType(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledBindingType(p0));
	}

	internal static Exception UnhandledConvert(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledConvert(p0));
	}

	internal static Exception UnhandledExpressionType(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledExpressionType(p0));
	}

	internal static Exception UnhandledUnary(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnhandledUnary(p0));
	}

	internal static Exception UnknownBindingType()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UnknownBindingType);
	}

	internal static Exception UserDefinedOpMustHaveConsistentTypes(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UserDefinedOpMustHaveConsistentTypes(p0, p1));
	}

	internal static Exception UserDefinedOpMustHaveValidReturnType(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.UserDefinedOpMustHaveValidReturnType(p0, p1));
	}

	internal static Exception LogicalOperatorMustHaveBooleanOperators(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.LogicalOperatorMustHaveBooleanOperators(p0, p1));
	}

	internal static Exception MethodDoesNotExistOnType(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MethodDoesNotExistOnType(p0, p1));
	}

	internal static Exception MethodWithArgsDoesNotExistOnType(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MethodWithArgsDoesNotExistOnType(p0, p1));
	}

	internal static Exception GenericMethodWithArgsDoesNotExistOnType(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.GenericMethodWithArgsDoesNotExistOnType(p0, p1));
	}

	internal static Exception MethodWithMoreThanOneMatch(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MethodWithMoreThanOneMatch(p0, p1));
	}

	internal static Exception PropertyWithMoreThanOneMatch(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.PropertyWithMoreThanOneMatch(p0, p1));
	}

	internal static Exception IncorrectNumberOfTypeArgsForFunc()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfTypeArgsForFunc);
	}

	internal static Exception IncorrectNumberOfTypeArgsForAction()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IncorrectNumberOfTypeArgsForAction);
	}

	internal static Exception ArgumentCannotBeOfTypeVoid()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.ArgumentCannotBeOfTypeVoid);
	}

	internal static Exception InvalidOperation(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InvalidOperation(p0));
	}

	internal static Exception OutOfRange(object p0, object p1)
	{
		return new ArgumentOutOfRangeException(System.Linq.Expressions.Strings.OutOfRange(p0, p1));
	}

	internal static Exception QueueEmpty()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.QueueEmpty);
	}

	internal static Exception LabelTargetAlreadyDefined(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.LabelTargetAlreadyDefined(p0));
	}

	internal static Exception LabelTargetUndefined(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.LabelTargetUndefined(p0));
	}

	internal static Exception ControlCannotLeaveFinally()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ControlCannotLeaveFinally);
	}

	internal static Exception ControlCannotLeaveFilterTest()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ControlCannotLeaveFilterTest);
	}

	internal static Exception AmbiguousJump(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.AmbiguousJump(p0));
	}

	internal static Exception ControlCannotEnterTry()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ControlCannotEnterTry);
	}

	internal static Exception ControlCannotEnterExpression()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ControlCannotEnterExpression);
	}

	internal static Exception NonLocalJumpWithValue(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.NonLocalJumpWithValue(p0));
	}

	internal static Exception ExtensionNotReduced()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.ExtensionNotReduced);
	}

	internal static Exception CannotCompileConstant(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CannotCompileConstant(p0));
	}

	internal static Exception CannotCompileDynamic()
	{
		return new NotSupportedException(System.Linq.Expressions.Strings.CannotCompileDynamic);
	}

	internal static Exception InvalidLvalue(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.InvalidLvalue(p0));
	}

	internal static Exception InvalidMemberType(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.InvalidMemberType(p0));
	}

	internal static Exception UnknownLiftType(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.UnknownLiftType(p0));
	}

	internal static Exception InvalidOutputDir()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InvalidOutputDir);
	}

	internal static Exception InvalidAsmNameOrExtension()
	{
		return new ArgumentException(System.Linq.Expressions.Strings.InvalidAsmNameOrExtension);
	}

	internal static Exception IllegalNewGenericParams(object p0)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.IllegalNewGenericParams(p0));
	}

	internal static Exception UndefinedVariable(object p0, object p1, object p2)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.UndefinedVariable(p0, p1, p2));
	}

	internal static Exception CannotCloseOverByRef(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.CannotCloseOverByRef(p0, p1));
	}

	internal static Exception UnexpectedVarArgsCall(object p0)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.UnexpectedVarArgsCall(p0));
	}

	internal static Exception RethrowRequiresCatch()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.RethrowRequiresCatch);
	}

	internal static Exception TryNotAllowedInFilter()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.TryNotAllowedInFilter);
	}

	internal static Exception MustRewriteToSameNode(object p0, object p1, object p2)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MustRewriteToSameNode(p0, p1, p2));
	}

	internal static Exception MustRewriteChildToSameType(object p0, object p1, object p2)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MustRewriteChildToSameType(p0, p1, p2));
	}

	internal static Exception MustRewriteWithoutMethod(object p0, object p1)
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.MustRewriteWithoutMethod(p0, p1));
	}

	internal static Exception TryNotSupportedForMethodsWithRefArgs(object p0)
	{
		return new NotSupportedException(System.Linq.Expressions.Strings.TryNotSupportedForMethodsWithRefArgs(p0));
	}

	internal static Exception TryNotSupportedForValueTypeInstances(object p0)
	{
		return new NotSupportedException(System.Linq.Expressions.Strings.TryNotSupportedForValueTypeInstances(p0));
	}

	internal static Exception HomogenousAppDomainRequired()
	{
		return new InvalidOperationException(System.Linq.Expressions.Strings.HomogenousAppDomainRequired);
	}

	internal static Exception TestValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.TestValueTypeDoesNotMatchComparisonMethodParameter(p0, p1));
	}

	internal static Exception SwitchValueTypeDoesNotMatchComparisonMethodParameter(object p0, object p1)
	{
		return new ArgumentException(System.Linq.Expressions.Strings.SwitchValueTypeDoesNotMatchComparisonMethodParameter(p0, p1));
	}

	internal static Exception PdbGeneratorNeedsExpressionCompiler()
	{
		return new NotSupportedException(System.Linq.Expressions.Strings.PdbGeneratorNeedsExpressionCompiler);
	}

	internal static Exception ArgumentNull(string paramName)
	{
		return new ArgumentNullException(paramName);
	}

	internal static Exception ArgumentOutOfRange(string paramName)
	{
		return new ArgumentOutOfRangeException(paramName);
	}

	internal static Exception NotSupported()
	{
		return new NotSupportedException();
	}

	internal static Exception OperatorNotImplementedForType(object p0, object p1)
	{
		return new NotImplementedException(System.Linq.Expressions.Strings.OperatorNotImplementedForType(p0, p1));
	}
}
