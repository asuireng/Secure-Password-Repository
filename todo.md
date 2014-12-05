# Remeber

Setup generic AJAX error, so all AJAX calls use it.
Include Ajax.BeginForm

# TO DO

- [ ] 1. Store password in encrypted cookie
- [ ] 2. Add password
- [ ] 3. Update password
- [ ] 4. Delete password
- [ ] 5. Approve account
- [ ] 6. Retreive password and details (AJAX)
- [ ] 7. Change account password
- [ ] 8. Reset password
- [ ] 9. Change database encryption key
- [ ] 10. Change system settings
- [ ] 11. Auto migrate database
- [ ] 12. Force SSL
- [ ] 13. Display user role
- [ ] 14. Edit user role

# DONE

## Remember:

Assume passwd cookie has been altered - verify PK after description - if bad, logout


## Implementation ideas (1)

Upon Login
> Generate key

> Encrypt passwd

> Write cookie

> Encrypt key with DPAPI

> Store key in app data


## Implementation ideas (2)