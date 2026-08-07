import { setupServer } from 'msw/node'
import { authHandlers } from './handlers/auth'
import { patientHandlers } from './handlers/patient'
import { adminHandlers } from './handlers/admin'
import { shopHandlers } from './handlers/shop'

export const server = setupServer(...authHandlers, ...patientHandlers, ...adminHandlers, ...shopHandlers)
