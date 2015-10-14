﻿using Components;
using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Component.Tests
{
	public class ComponentTests
	{
		[Fact]
		public static void WhenServiceAddedThenCanResolveComponentSystem()
		{
			new ServiceCollection()
				.UseComponentSystem()
				.BuildServiceProvider()
				.GetService<ComponentSystem>()
				.Should().NotBeNull();
		}

		private static IServiceProvider CreateProvider()
			=> new ServiceCollection()
				.UseComponentSystem()
				.BuildServiceProvider();

		private static ComponentSystem CreateTarget(IServiceProvider provider = null)
			=> (provider ?? CreateProvider())
				.GetRequiredService<ComponentSystem>();

		[Fact]
		public static void WhenAssigningComponentToNullEntityThenThrowsException()
		{
			Action act = () => CreateTarget().Assign((object) null, new object());
			act.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public static void WhenAssigningNullComponentToEntityThenThrowsException()
		{
			Action act = () => CreateTarget().Assign(new object(), (object)null);
			act.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public static void WhenSameComponentTypeAssignedToSameEntityInstanceThenThrowsException()
		{
			var entity = new object();
			var provider = CreateProvider();
			Action act = () => CreateTarget(provider).Assign(entity, new object());
			act.ShouldNotThrow<Exception>();
			act.ShouldThrow<ComponentAlreadyAssignedException>();
		}

		[Fact]
		public static void WhenTryingToAssignComponentToNullEntityThenReturnsFalse()
		{
			CreateTarget().TryAssign((object)null, new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenTryingToAssignNullComponentToEntityThenReturnsFalse()
		{
			CreateTarget().TryAssign(new object(), (object)null)
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenTryingToAssignComponentToEntityThenReturnsTrue()
		{
			CreateTarget().TryAssign(new object(), new object())
				.Should().BeTrue();
		}

		[Fact]
		public static void WhenTryingToAssignSameComponentTypeToSameEntityInstanceThenReturnsFalse()
		{
			var entity = new object();
			var provider = CreateProvider();
			CreateTarget(provider).TryAssign(entity, new object())
				.Should().BeTrue();
			CreateTarget(provider).TryAssign(entity, new object())
				.Should().BeFalse();
		}

		[Fact]
		public static void WhenTryingToAssignSameComponentsToDifferentEntityInstancesThenReturnsTrue()
		{
			var provider = CreateProvider();
			var component = new object();
			CreateTarget(provider).TryAssign(new object(), component)
				.Should().BeTrue();
			CreateTarget(provider).TryAssign(new object(), component)
				.Should().BeTrue();
		}

		[Fact]
		public static void WhenRetrievingStateForEntityNeverAssignedToThenDoesNotThrowException()
		{
			Action act = () => CreateTarget().StateFor(new object());
			act.ShouldNotThrow();
		}

		[Fact]
		public static void WhenRetrievingStateForEntityNeverAssignedToThenDoesNotReturnNull()
		{
			CreateTarget().StateFor(new object())
				.Should().NotBeNull();
		}

		[Fact]
		public static void WhenRetrievingComponentNeverAssignedToEntityThenDoesNotThrowException()
		{
			Action act = () => CreateTarget().StateFor(new object()).Get<object>();
			act.ShouldNotThrow();
		}

		[Fact]
		public static void WhenRetrievingComponentAssignedToSameEntityInstanceThenReturnsComponent()
		{
			var entity = new object();
			var component = new object();
			var target = CreateTarget();
			target.Assign(entity, component);

			target.StateFor(entity).Get<object>()
				.Should().Be(component);
		}

		[Fact]
		public static void WhenRetrievingComponentAssignedToDifferentEntityInstanceThenReturnsDefaultComponent()
		{
			var target = CreateTarget();
			target.Assign(new object(), new object());

			target.StateFor(new object()).Get<object>()
				.Should().Be(default(object));
		}
	}
}
