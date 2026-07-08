import { mergeApplicationConfig, ApplicationConfig, NgZone } from '@angular/core';
import { provideServerRendering, withRoutes } from '@angular/ssr';
import { appConfig } from './app.config';
import { serverRoutes } from './app.routes.server';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering(withRoutes(serverRoutes)),
    {
      provide: NgZone,
      useValue: new NgZone({ enableLongStackTrace: false })
    }
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
