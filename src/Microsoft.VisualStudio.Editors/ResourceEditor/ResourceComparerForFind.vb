﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Option Explicit On
Option Strict On
Option Compare Binary

Namespace Microsoft.VisualStudio.Editors.ResourceEditor

    ''' <summary>
    ''' This is an Icomparer implementation used to sort Resources for Find/Replace purposes.  It sorts according
    '''   to both category (in a given order) and resource name.
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class ResourceComparerForFind
        Implements IComparer(Of Resource)

        'A dictionary that maps a Category to its sort order
        Private ReadOnly _categoryToCategoryOrderHash As New Dictionary(Of Category, Integer)

        'All categories included in the search
        Private ReadOnly _categories As CategoryCollection



        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="OrderedCategories">List of all categories, in order of desired search order</param>
        ''' <remarks></remarks>
        Public Sub New(OrderedCategories As CategoryCollection)
            Debug.Assert(OrderedCategories IsNot Nothing)
            _categories = OrderedCategories

            'Fill our hashtable with the desired category search order.  Map is from Category to its search order
            '  (lower has higher priority)

            Dim CategoryOrder As Integer = 0
            For Each Category As Category In OrderedCategories
                _categoryToCategoryOrderHash(Category) = CategoryOrder
                CategoryOrder += 1
            Next
            Debug.Assert(_categoryToCategoryOrderHash.Count = OrderedCategories.Count)
        End Sub


        ''' <summary>
        ''' Sorts a <see cref="List(Of Resource)"/> for UI purposes
        ''' </summary>
        ''' <param name="Resources"><see cref="List(Of Resource)"/> to sort (will be sorted in place)</param>
        ''' <remarks></remarks>
        Public Sub SortResources(Resources As List(Of Resource))
            Resources.Sort(Me)
        End Sub


        ''' <summary>
        ''' Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        ''' </summary>
        ''' <param name="Resource1">First object to compare.</param>
        ''' <param name="Resource2">Second object to compare.</param>
        ''' <returns>-1, 0 or 1, depending on whether x is less than, equal to or greater than y, respectively.</returns>
        ''' <remarks>This function gets called by ArrayList.Sort for each pair of resources to be sorted.</remarks>
        Private Function Compare(Resource1 As Resource, Resource2 As Resource) As Integer Implements IComparer(Of Resource).Compare
            Dim category1 As Category = Resource1.GetCategory(_categories)

            'First compare by category
            Dim Resource1CategoryOrder As Integer = _categoryToCategoryOrderHash(category1)
            Dim Resource2CategoryOrder As Integer = _categoryToCategoryOrderHash(Resource2.GetCategory(_categories))

            If Resource1CategoryOrder > Resource2CategoryOrder Then
                Return 1
            ElseIf Resource1CategoryOrder < Resource2CategoryOrder Then
                Return -1
            End If

            '... then by order defined in the Category
            Return category1.Compare(Resource1, Resource2)
        End Function
    End Class

End Namespace
