# Security

- Avoid running `kvtool` with `--show-values` in shared terminals or recorded logs; only reveal secrets in audited, trusted environments.
- Grant Key Vault access using least-privilege principals (use scoped RBAC roles and dedicated Service Principals) and rotate credentials frequently.