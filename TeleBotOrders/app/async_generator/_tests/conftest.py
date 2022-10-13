import pytest
from functools import wraps, partial
import inspect
import types


@types.coroutine
def mock_sleep():
    yield "mock_sleep"


# Wrap any 'async def' tests so that they get automatically iterated.
# We used to use pytest-asyncio as a convenient way to do this, but nowadays
# pytest-asyncio uses us! In addition to it being generally bad for our test
# infrastructure to depend on the code-under-test, this totally messes up
# coverage info because depending on pytest's plugin load order, we might get
# imported before pytest-cov can be loaded and start gathering coverage.
@pytest.hookimpl(tryfirst=True)
def pytest_pyfunc_call(pyfuncitem):
    if inspect.iscoroutinefunction(pyfuncitem.obj):
        fn = pyfuncitem.obj

        @wraps(fn)
        def wrapper(**kwargs):
            coro = fn(**kwargs)
            try:
                while True:
                    value = coro.send(None)
                    if value != "mock_sleep":  # pragma: no cover
                        raise RuntimeError(
                            "coroutine runner confused: {!r}".format(value)
                        )
            except StopIteration:
                pass

        pyfuncitem.obj = wrapper
