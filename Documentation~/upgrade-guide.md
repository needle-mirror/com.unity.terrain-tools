# Upgrade Terrain Tools from 3.0.2 to 4.0.0
In the Terrain Tools package, some features work differently between versions. This document helps you upgrade Terrain Tools from 3.0.2 to 4.0.0.

## Experimental API upgrade
From 4.0.0, Terrain APIs are no longer experimental. The namespaces were changed to reflect this.

| Old namespace                  | New namespace          |
| ----------------------------------- | ---------------------- |
| UnityEditor.Experimental.TerrainAPI | UnityEditor.TerrainAPI |
| UnityEngine.Experimental.TerrainAPI | UnityEngine.TerrainAPI |

## API scope
A portion of the Terrain Tools API access modifiers were changed from public to internal.

