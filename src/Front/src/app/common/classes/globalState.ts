import { computed, Signal, signal } from '@angular/core';

export class GlobalState<T> {
  state = signal({} as T);

  public set<k extends keyof T>(key: k, data: T[k]) {
    this.state.update((currentValue: T) => ({
      ...currentValue,
      [key]: data,
    }));
  }

  public setState(partialState: Partial<T>) {
    this.state.update((currentValue: T) => ({
      ...currentValue,
      ...partialState,
    }));
  }

  public select<k extends keyof T>(key: k) {
    return computed(() => this.state()[key]);
  }

  public get(): Signal<T> {
    return computed(() => this.state());
  }

  public reset() {
    this.state.set({} as T);
  }
}
