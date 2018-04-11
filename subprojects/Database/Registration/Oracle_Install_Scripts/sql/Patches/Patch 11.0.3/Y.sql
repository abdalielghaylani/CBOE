--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

-- RLS activated. Activating Registry Level Projects or Batch Level Projects

PROMPT Row-Level security is active. What is the project level you want to activate?
ACCEPT LevelRLS CHAR DEFAULT 'R' PROMPT '[ Registry Level Projects(R) | Batch Level Projects(B) ] (R):'
@@"sql\Patches\Patch 11.0.3\&&LevelRLS"